using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CoreLaunching
{
    public class LoggerLoader
    {
        public LoggerLoader(Process game)
        {
            game.Exited += Game_Exited;
        }

        private void Game_Exited(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
