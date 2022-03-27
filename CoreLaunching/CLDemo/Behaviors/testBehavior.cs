using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace CLDemo.Behaviors
{
    internal class TestBehavior:Behavior<Button>
    {
        public TestBehavior()
        {

        }

        protected override void OnAttached()
        {
            base.OnAttached();
            var btn = this.AssociatedObject;
        }
    }
}
