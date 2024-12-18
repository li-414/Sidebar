
namespace Sidebar.Helpers
{
    internal class ScreenHelper
    {
        public void ScreenShow(nint handle)
        {
            var screens = Screen.AllScreens;

            if (screens.Length > 1)
            {
                // 有两个或以上显示器，选择第二个显示器
                var secondScreen = screens.FirstOrDefault(x => x.Bounds.X > 0);
                if (secondScreen != null)
                {
                    FullScreenOnScreen(secondScreen, handle);
                }
            }
            else
            {
                // 只有一个显示器
                var primaryScreen = screens.FirstOrDefault(x => x.Bounds.X == 0 && x.Bounds.Y == 0);
                FullScreenOnScreen(primaryScreen, handle);
            }
        }

        private void FullScreenOnScreen(Screen? targetScreen, nint handle)
        {
            //WindowHelper.SetWindowNoStyle(handle);
            (int width, int height) = WindowHelper.GetWindowSize(handle);

            if (targetScreen == null) return;
            var workingArea = targetScreen.WorkingArea;


            int showWidth = workingArea.Width - width;
            int showHeight = workingArea.Height - height;
            int showX = showWidth / 2 + workingArea.Left;
            int showY = showHeight / 2 + workingArea.Top;

            WindowHelper.MoveWindow(
                handle,
                (int)workingArea.Left,
                (int)workingArea.Top,
                (int)workingArea.Width,
                (int)workingArea.Height,
                true
            );
        }




        public void ScreenShowLeft(nint handle)
        {
            var screens = Screen.AllScreens;

            if (screens.Length > 1)
            {
                // 有两个或以上显示器，选择第二个显示器
                var secondScreen = screens.FirstOrDefault(x => x.Bounds.X > 0);
                if (secondScreen != null)
                {
                    LeftScreenOnScreen(secondScreen, handle);
                }
            }
            else
            {
                // 只有一个显示器
                var primaryScreen = screens.FirstOrDefault(x => x.Bounds.X == 0 && x.Bounds.Y == 0);
                LeftScreenOnScreen(primaryScreen, handle);
            }
        }

        private void LeftScreenOnScreen(Screen? primaryScreen, nint handle)
        {
            if (primaryScreen == null) return;
            var workingArea = primaryScreen.WorkingArea;

            WindowHelper.MoveWindow(
              handle,
              (int)workingArea.Left + 5,
              (int)workingArea.Top,
              (int)workingArea.Width / 2 - 10,
              (int)workingArea.Height,
              true
          );
        }



        public void ScreenShowRight(nint handle)
        {
            var screens = Screen.AllScreens;

            if (screens.Length > 1)
            {
                // 有两个或以上显示器，选择第二个显示器
                var secondScreen = screens.FirstOrDefault(x => x.Bounds.X > 0);
                if (secondScreen != null)
                {
                    RightScreenOnScreen(secondScreen, handle);
                }
            }
            else
            {
                // 只有一个显示器
                var primaryScreen = screens.FirstOrDefault(x => x.Bounds.X == 0 && x.Bounds.Y == 0);
                RightScreenOnScreen(primaryScreen, handle);
            }
        }

        private void RightScreenOnScreen(Screen? primaryScreen, nint handle)
        {
            if (primaryScreen == null) return;
            var workingArea = primaryScreen.WorkingArea;

            WindowHelper.MoveWindow(
              handle,
              (int)workingArea.Left + (int)workingArea.Width / 2 + 5,
              (int)workingArea.Top,
              (int)workingArea.Width / 2 - 10,
              (int)workingArea.Height,
              true
          );
        }
    }
}
