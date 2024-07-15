
using Interpreter_cs.MonkeyObjects;

namespace Interpreter_cs.MonkeyEvaluator {
    public class Builtins {
        
        internal Dictionary<string, BuildIn> builtins = new Dictionary<string, BuildIn>();

        public Builtins() {
            builtins = new Dictionary<string, BuildIn>() { 
                {"len", new BuildIn() { fn = Builds.len } },
            };
        }

    }
}
