﻿using Crainiate.Diagramming;
using Crainiate.Diagramming.Flowcharting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TestingLast.Nodes
{

    public abstract class BaseNode : Crainiate.Diagramming.OnShapeClickListener
    {
        static Controller controller;
        static Form1 form;
        //public static List<BaseNode> nodes = new List<BaseNode>();      
        static Model model;
        private ConnectorNode outConnector;
        Form dialog;
        String statement;
        Shape shape;
        BaseNode parentNode;
        FlowchartStencil stencil;
        private String shapeTag;
        private String connectorTag;
        static int counter;
        protected int shiftY = 95;
        protected PointF nodeLocation;//= new PointF(0, 0);
        protected float moreShift = 0;
        public Form Dialog
        {
            get
            {
                return dialog;
            }

            set
            {
                dialog = value;
            }
        }

        public string Statement
        {
            get
            {
                return statement;
            }

            set
            {
                statement = value;
            }
        }

        public virtual Shape connectedShape()
        {
            return Shape;
        }

        public Shape Shape
        {
            get
            {
                return shape;
            }

            /*  set
              {
                  shape = value;
              } */
        }

        public FlowchartStencil Stencil
        {
            get
            {
                return stencil;
            }

            set
            {
                stencil = value;
            }
        }

        public static Model Model
        {
            get
            {
                return model;
            }

            set
            {
                model = value;
            }
        }

        virtual public PointF NodeLocation
        {
            get
            {
                return nodeLocation;
            }

            set
            {
                nodeLocation = value;
                connectedShape().Location = value;
            }
        }

        public ConnectorNode OutConnector
        {
            get
            {
                return outConnector;
            }

            set
            {
                outConnector = value;
            }
        }

        public static Form1 Form
        {
            get
            {
                return form;
            }

            set
            {
                form = value;
            }
        }

        public string Name { get; internal set; }

        public BaseNode ParentNode
        {
            get
            {
                return parentNode;
            }

            set
            {
                parentNode = value;
            }
        }

        internal static Controller Controller
        {
            get
            {
                return controller;
            }

            set
            {
                controller = value;
            }
        }

        public string ShapeTag
        {
            get
            {
                return shapeTag;
            }

            set
            {
                shapeTag = value;
            }
        }

        public string ConnectorTag
        {
            get
            {
                return connectorTag;
            }

            set
            {
                connectorTag = value;
            }
        }

        public BaseNode()
        {
            if (Controller == null)
                throw new Exception("Controller Must Be set First");
            shape = new Shape();
            Shape.Label = new Crainiate.Diagramming.Label();
            Shape.Label.Color = Color.White;
            Shape.KeepAspect = false;
            shape.ShapeListener = this;
            Shape.AllowMove = Shape.AllowScale = Shape.AllowRotate = Shape.AllowSnap = false;
            Shape.Size = new SizeF(80, 50);
            Shape.KeepAspect = false;
            Stencil = (FlowchartStencil)Singleton.Instance.GetStencil(typeof(FlowchartStencil));
            Shape.Label.Color = Color.White;
            OutConnector = new ConnectorNode(this);
            counter++;
            ShapeTag = "Shape_" + counter.ToString();
            ConnectorTag = "Connector_" + counter.ToString();
           
            
        }
        public BaseNode(PointF location): this()
        {
            NodeLocation = location;
        }
        virtual public void setText(String label) {
            Shape.Label = new Crainiate.Diagramming.Label(label);
            SizeF size;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                size = g.MeasureString(label, Shape.Label.Font);
            }
            // SizeF size =TextRenderer.MeasureText("Input SAFSAF ASFSAFS ASSAFFS ", Singleton.Instance.DefaultFont);
            Shape.Size = new SizeF(size.Width + 70, Shape.Size.Height);
            Shape.Label.Color = Color.White;
        }
        virtual public void addToModel()
        {
            Controller.addToModel(this);
           
            
        }
        virtual public void removeFromModel()
        {
            Controller.removeNode(this);
           
        }

        


        public abstract void onShapeClicked();
        public void attachNode(BaseNode newNode)
        {

              if (this is TerminalNode && newNode is TerminalNode ||
                  this is HolderNode && newNode is HolderNode)
              {
                  if (newNode.connectedShape() == null)
                  {
                      //do nothing
                  }
                  if (this.connectedShape() == null)
                  {
                      //donothing
                  }
                  OutConnector.EndNode = newNode;
                  newNode.NodeLocation = this.NodeLocation;
                  newNode.shiftDown(0);
                  return;

              }
           
            BaseNode oldOutNode = OutConnector.EndNode;
            OutConnector.EndNode = newNode;
            newNode.OutConnector.EndNode = oldOutNode;
            float x = this.NodeLocation.X;
            if (this.NodeLocation.X != oldOutNode.NodeLocation.X) {
                if (this is HolderNode)
                    x = oldOutNode.NodeLocation.X;
                else if (oldOutNode is HolderNode)
                    x = this.NodeLocation.X;
            }
          //  if (this.NodeLocation.X == oldOutNode.NodeLocation.X)
            {
                  /*if (this is HolderNode) {
                      float x = this.NodeLocation.X - newNode.Shape.Width / 2;
                      newNode.NodeLocation = new PointF(x, oldOutNode.NodeLocation.Y);
                  }
                  else*/
                  //give the new node the same x as this node and y as the node used to be in it's place
                  newNode.NodeLocation = new PointF(x, oldOutNode.NodeLocation.Y) ;
                  oldOutNode.shiftDown(0);
             
                  controller.balanceNodes(newNode);
             }
            
        }
        
     
        
        public virtual void attachNode(BaseNode newNode, ConnectorNode connectorNode)
        {
            attachNode(newNode);

        }
       
        virtual public void shiftDown(float moreShift=0)
        {
           // this.moreShift = moreShift;
            
            if (connectedShape() != null)
                NodeLocation = new PointF(connectedShape().Location.X, connectedShape().Location.Y + shiftY+moreShift);
            
            if (!(this is HolderNode)&& OutConnector.EndNode != null)
                OutConnector.EndNode.shiftDown(moreShift);
        }
        //used to shift nodes up offsetY is how much to shift up
        public void shiftUp( float offsetY )
        {
            
            NodeLocation = new PointF(NodeLocation.X, NodeLocation.Y - offsetY);
            if (OutConnector.EndNode == null || this is DecisionNode)
                return;
            if (this is HolderNode) //what about middleNode shift
            {
                 //to decide shifting middle node or not
                if (this.ParentNode is IfElseNode)
                {
                    IfElseNode pNode = this.ParentNode as IfElseNode;
                    PointF preLocation = pNode.MiddleNode.NodeLocation;
                    pNode.balanceHolderNodes();
                    if (pNode.MiddleNode.NodeLocation.Y == preLocation.Y)
                        return; //thus don't shift the node after parent node
                }
                //shifting middle node 
                else if (this.ParentNode is IfNode)
                {

                    IfNode pNode = (this.ParentNode as IfNode);
                    pNode.MiddleNode.NodeLocation = new PointF(pNode.MiddleNode.connectedShape().Location.X, pNode.BackNode.NodeLocation.Y);

                }
                this.ParentNode.OutConnector.EndNode.shiftUp(offsetY);
               
            }
            else
                OutConnector.EndNode.shiftUp(offsetY);
                
            
        }
        public void shiftRight(int distance) {
            NodeLocation = new PointF(NodeLocation.X + distance,NodeLocation.Y);
        }
    }
}
