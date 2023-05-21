namespace Fluxcp;
// AST (Abstract Syntax Tree). Contains the root node.
//example of AST for the code:
//    use FCP.Main;
//    void Main() 
//    {
//        f32 myFloat = 1.5;
//        Standart.Out.Print(str.Parse(myFloat));
//    }
//

//                  root (Starting bound node)
//                 /    \    
//     useStatement      mainFunc
//       /              /   |   \
//   libName    returnType body name
//                        /    \
//                 varNode     TODO
//                /   |   \
//            level  name  type
//
//
// 
public class SyntaxTree 
{
    public SyntaxNode Root {get; private set;}
    public SyntaxTree()
    {
        Root = new ProgramBound();
    }
    private class ProgramBound : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            // all the body are children
            if (Next != null)
                yield return Next!;
        }
    }
}