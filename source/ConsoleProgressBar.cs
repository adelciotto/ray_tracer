using System;
using System.Text;
using System.Threading;

namespace RayTracer
{
    class ConsoleProgressBar : IDisposable
    {
        private const int maxBlocks = 10;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 33);

        private readonly Timer _timer;
        private string _prefix;
        private float _progress;
        private string _currentText = string.Empty;
        private bool _disposed;

        public ConsoleProgressBar(string prefix)
        {
            _prefix = prefix;
            _timer = new Timer(TimerHandler);

            Console.CursorVisible = false;

            ResetTimer();
        }

        public void Report(float progress)
        {
            progress = Math.Clamp(progress, 0.0f, 1.0f);
            Interlocked.Exchange(ref _progress, progress); 
        }

        public void Dispose()
        {
            lock (_timer)
            {
                _disposed = true;
                UpdateText("Done!\n");

                Console.CursorVisible = true;
            }
        }

        private void TimerHandler(object state)
        {
            lock (_timer)
            {
                if (_disposed)
                    return;

                int progressBlockCount = (int)(_progress * maxBlocks);
                int percent = (int)(_progress * 100);
                string text = string.Format(
                    "{0}: [{1}{2}] {3, 3}%",
                    _prefix,
                    new string('#', progressBlockCount),
                    new string('-', maxBlocks - progressBlockCount),
                    percent
                );
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            // When the progress bar is complete, clear the text and write the 'Done!' text.
            if (_disposed)
            {
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.BufferWidth)); 
                Console.SetCursorPosition(0, currentLineCursor);

                Console.Write(text);
                return;
            }    

            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(_currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength]) {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            Console.Write(outputBuilder);
            _currentText = text;
        }

        private void ResetTimer()
        {
            _timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
        }
    }
}
