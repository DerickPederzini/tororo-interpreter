
using Interpreter_cs.MonkeyObjects;

namespace Interpreter_cs.MonkeyEvaluator {
    public class Builtins {
        internal Dictionary<string, BuildIn> builtins = new Dictionary<string, BuildIn>();

        public Builtins() {
            builtins = new Dictionary<string, BuildIn>() { 
                {"len", new BuildIn() { fn = Builds.len } },
                {"first", new BuildIn() { fn = Builds.first } },
                {"last", new BuildIn() { fn = Builds.last } },
                {"rest", new BuildIn() { fn = Builds.rest } },
                {"push", new BuildIn() { fn = Builds.push} }
            };
        }
    }
}
