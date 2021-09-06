using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Messager.Extensions
{
    public static class SmoothProgressBar
    {

        public static void SetPercent(this System.Windows.Controls.ProgressBar bar, double percentage, float dur = 0.5f) {
            DoubleAnimation anim = new DoubleAnimation(percentage, TimeSpan.FromSeconds(dur));
            bar.BeginAnimation(System.Windows.Controls.ProgressBar.ValueProperty, anim);
        }
    }
}
