namespace DetroitRunner.Tests;

public class Abstract
{
    
    public abstract class Shape
    {
        private string Name { get; }
        
        protected Shape(string name)=> Name = name;
        
        protected abstract double Area();
        
        public virtual string Describe() => $"{Name} with area {Area()}";
    }


    class Circle : Shape
    {
        public readonly double _r;
        public Circle(double r): base("Circle") => _r = r;
        protected override double Area() => Math.PI * _r * _r;
    }

    class Rectangle : Shape
    {
        public readonly double _w, _h;
        public Rectangle(double w, double h) : base("Rectangle") => (_w, _h) = (w, h);
        protected override double Area() => _w * _h;
        
    }

    
    [Fact]
   public void AbstractTest()
   {
       Shape[] shapes = { new Circle(2), new Rectangle(3, 4) };
       foreach(var s in shapes)
       {
           Console.WriteLine(s.Describe());
       }
   }
    
}
