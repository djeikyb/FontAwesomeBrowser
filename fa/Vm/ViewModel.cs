using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using fa.Data;
using ObservableCollections;
using R3;
using Serilog.Events;

namespace fa.Vm;

public class JsonSvgVm(JsonSvg jsonSvg)
{
    public BindableReactiveProperty<bool> IsSelected { get; } = new(false);

    public JsonIcon Icon => jsonSvg.Icon;

    public string SearchTermsCsv { get; } = string.Join(", ", jsonSvg.Icon.Search.terms);

    /// This will be a single value. Over in <see cref="JsonIcon.Styles"/> it
    /// indicates all styles supported. Here it indicates the style of this
    /// specific representation of an icon.
    public Styles Style => jsonSvg.Style;

    public string Path => jsonSvg.Path;
}

public class ViewModel
{
    public ViewModel(Icons icons)
    {
        ObservableList<JsonSvgVm> _icons = new(icons.List
            .Select(x =>
            {
                var a = new JsonSvg[BitOperations.PopCount((uint)x.Styles)];
                var i = -1;

                // The ¡bang! to suppress nulls is correct SO LONG as the
                // invariant holds in the original json from FontAwesome. For
                // every style in the "styles" array, there should be an object
                // of that style in the "svg" dictionary.
                if (x.Styles.HasFlag(Styles.solid)) a[++i] = x.Svg.Solid!;
                if (x.Styles.HasFlag(Styles.regular)) a[++i] = x.Svg.Regular!;
                if (x.Styles.HasFlag(Styles.light)) a[++i] = x.Svg.Light!;
                if (x.Styles.HasFlag(Styles.thin)) a[++i] = x.Svg.Thin!;
                if (x.Styles.HasFlag(Styles.brands)) a[++i] = x.Svg.Brands!;

                return a;
            })
            .SelectMany(x => x)
            .Select(x => new JsonSvgVm(x)));

        var v = _icons.CreateView(x => x);

        SearchTerm = new();

        StyleList = Enum.GetNames<Styles>().Prepend("Any style").ToList();
        StyleListItemSelected = new("Any style");
        Observable.Merge(StyleListItemSelected.AsUnitObservable(), SearchTerm.AsUnitObservable())
            .Debounce(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                v.AttachFilter(new Filter(StyleListItemSelected.Value, SearchTerm.Value));
            });

        IconList = v.ToNotifyCollectionChanged();

        PreviewItem = new();
        PreviewStyleSelected = new();
        PreviewStyleSelected.Subscribe(maybeStyle =>
        {
            if (maybeStyle is null) return;
            var currentIcon = PreviewItem.Value?.Icon;
            if (currentIcon is null) return;
            var style = Enum.Parse<Styles>(maybeStyle);
            var foo = style switch
            {
                Styles.solid => currentIcon.Svg.Solid,
                Styles.regular => currentIcon.Svg.Regular,
                Styles.light => currentIcon.Svg.Light,
                Styles.thin => currentIcon.Svg.Thin,
                Styles.brands => currentIcon.Svg.Brands,
                _ => throw new ArgumentOutOfRangeException()
            };
            PreviewItem.Value = foo is not null ? new JsonSvgVm(foo) : null;
        });

        IconListItemSelected = new BindableReactiveProperty<JsonSvgVm?>();
        IconListItemSelected.Pairwise().Subscribe(x =>
        {
            var (previous, current) = x;
            if (previous != null) previous.IsSelected.Value = false;
            if (current != null) current.IsSelected.Value = true;
            PreviewItem.Value = current;
            PreviewStyleSelected.Value = current?.Style.ToString();
        });

        LogViews = App.LogsSink.Logs.ToNotifyCollectionChanged(SynchronizationContextCollectionEventDispatcher.Current);
        TintOpacity = new(1m);
        MaterialOpacity = new(1m);
    }

    private class Filter(string style, string? term) : ISynchronizedViewFilter<JsonSvgVm>
    {
        public bool IsMatch(JsonSvgVm value)
        {
            // predicate 1
            if (term is { Length: > 0 })
            {
                if (value.Icon.Label.Contains(term, StringComparison.InvariantCultureIgnoreCase))
                    goto matchedSearchTerm;
                var haystack = value.Icon.Search.terms;
                foreach (var strawOrNeedle in haystack)
                {
                    if (strawOrNeedle.Contains(term, StringComparison.InvariantCultureIgnoreCase))
                        goto matchedSearchTerm;
                }

                return false;
            }

            // predicate 2
            matchedSearchTerm:
            if (style.Equals("Any style")) return true;
            return value.Style == Enum.Parse<Styles>(style);
        }
    }

    public BindableReactiveProperty<string?> SearchTerm { get; }

    public BindableReactiveProperty<JsonSvgVm?> PreviewItem { get; }
    public BindableReactiveProperty<string?> PreviewStyleSelected { get; }

    public BindableReactiveProperty<string> StyleListItemSelected { get; }
    public List<string> StyleList { get; set; }

    public INotifyCollectionChangedSynchronizedViewList<JsonSvgVm> IconList { get; }

    public BindableReactiveProperty<decimal> TintOpacity { get; }
    public BindableReactiveProperty<decimal> MaterialOpacity { get; }

    public BindableReactiveProperty<JsonSvgVm?> IconListItemSelected { get; }

    public INotifyCollectionChangedSynchronizedViewList<LogEvent> LogViews { get; }
}
