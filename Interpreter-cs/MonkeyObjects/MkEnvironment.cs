
using FluentAssertions;

namespace Interpreter_cs.MonkeyObjects; 
public class MkEnvironment {
    internal Dictionary<string, ObjectInterface> environment { get; set; }
    internal MkEnvironment ? outer;

    public MkEnvironment() {
        environment = new Dictionary<string, ObjectInterface>();
    }

    public MkEnvironment newEnclosedEnvironment(MkEnvironment outer) {
        var environment = new MkEnvironment();
        environment.outer = outer;
        return environment;
    }

    public ObjectInterface getEnvironment(string name) {
        var get = environment[name];
        get.Should().NotBeNull();
        if (outer != null) {
            get = outer.getEnvironment(name);
        }
        return get;
    }
}
