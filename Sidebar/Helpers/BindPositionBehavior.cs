using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace Sidebar.Helpers
{
    public class BindPositionBehavior : Behavior<FrameworkElement>
    {
        public Action<Rect> BindPosition
        {
            get { return (Action<Rect>)GetValue(BindPositionProperty); }
            set { SetValue(BindPositionProperty, value); }
        }

        public static readonly DependencyProperty BindPositionProperty =
            DependencyProperty.Register(
                nameof(BindPosition),
                typeof(Action<Rect>),
                typeof(BindPositionBehavior),
                new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.LayoutUpdated += OnLayoutUpdated;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.LayoutUpdated -= OnLayoutUpdated;
        }


        private Rect _previousRect = Rect.Empty;
        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            var element = AssociatedObject;
            var parentWindow = Window.GetWindow(element);
            if (parentWindow != null)
            {
                var rect = element.TransformToAncestor(parentWindow)
                    .TransformBounds(new Rect(element.RenderSize));

                // 检查位置或大小是否有变化
                if (rect != _previousRect)
                {
                    _previousRect = rect;
                    BindPosition?.Invoke(rect);
                }
            }
        }
    }

}
