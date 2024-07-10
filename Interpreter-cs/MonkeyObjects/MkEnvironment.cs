
namespace Interpreter_cs.MonkeyObjects; 
public class MkEnvironment {
    internal Dictionary<string, ObjectInterface> environment { get; set; }
    public MkEnvironment() {
        environment = new Dictionary<string, ObjectInterface>();
    }
}
