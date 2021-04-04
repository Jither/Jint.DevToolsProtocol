using Jint.Runtime.Debugger;
using System;
using System.Threading;

namespace Jint.DevToolsProtocol.Protocol
{
    public class Debugger
    {
        private readonly Engine _engine;
        private readonly Agent _agent;

        private readonly ManualResetEvent _waitForInterface = new ManualResetEvent(false);
        private StepMode _nextStep;
        private bool _isRunning;
        private bool _isStepping;

        public event EventHandler<DebugInformation> Paused;
        public event EventHandler Resumed;

        public Debugger(Agent agent, Engine engine)
        {
            Id = Guid.NewGuid().ToString();

            _engine = engine;
            _agent = agent;

            _engine.DebugHandler.Step += DebugHandler_Step;
            _engine.DebugHandler.Break += DebugHandler_Break;
            _engine.Parsed += Engine_Parsed;

            _isStepping = true;
        }

        public string Id { get; }

        private void Engine_Parsed(object sender, SourceParsedEventArgs e)
        {
            _agent.RuntimeData.AddSource(e.SourceId, $"jint://{e.SourceId}", e.Source, e.Ast);
            _agent.DebuggerDomain.SendScripts();
        }

        private StepMode DebugHandler_Break(object sender, DebugInformation e)
        {
            return OnPause(e);
        }

        private StepMode DebugHandler_Step(object sender, DebugInformation e)
        {
            if (!_isStepping)
            {
                // If we aren't stepping, immediately step into the next statement.
                return StepMode.Into;
            }
            return OnPause(e);
        }

        private StepMode OnPause(DebugInformation debugInformation)
        {
            Stop();

            _isRunning = false;

            Paused?.Invoke(this, debugInformation);

            _waitForInterface.WaitOne();
            _waitForInterface.Reset();

            _isRunning = true;

            Resumed?.Invoke(this, EventArgs.Empty);

            return _nextStep;
        }

        public void Pause()
        {
            _waitForInterface.Reset();
        }

        public void Run()
        {
            _isStepping = false;
            _nextStep = StepMode.Into;
            _waitForInterface.Set();
        }

        public void Stop()
        {
            _isStepping = true;
        }

        public void StepInto()
        {
            _nextStep = StepMode.Into;
            _waitForInterface.Set();
        }

        public void StepOver()
        {
            _nextStep = StepMode.Over;
            _waitForInterface.Set();
        }

        public void StepOut()
        {
            _nextStep = StepMode.Out;
            _waitForInterface.Set();
        }
    }
}
