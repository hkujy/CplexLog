// --------------------------------------------------------------------------
// File: QPex1.cs
// Version 12.8.0
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
// 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
// Copyright IBM Corporation 2003, 2017. All Rights Reserved.
//
// US Government Users Restricted Rights - Use, duplication or
// disclosure restricted by GSA ADP Schedule Contract with
// IBM Corp.
// --------------------------------------------------------------------------
//
// QPex1.cs - Entering and optimizing a QP problem

using ILOG.Concert;
using ILOG.CPLEX;


public class QPex1 {
   public static void Main(string[] args) {
      try {
         Cplex cplex = new Cplex();
         ILPMatrix lp = PopulateByRow(cplex);


         if ( cplex.Solve() ) {
            double[] x     = cplex.GetValues(lp);
            double[] dj    = cplex.GetReducedCosts(lp);
            double[] pi    = cplex.GetDuals(lp);
            double[] slack = cplex.GetSlacks(lp);

            System.Console.WriteLine("Solution status = " + cplex.GetStatus());
            System.Console.WriteLine("Solution value  = " + cplex.ObjValue);

            int nvars = x.Length;
            for (int j = 0; j < nvars; ++j) {
               System.Console.WriteLine("Variable   " + j +
                                        ": Value = " + x[j] +
                                        " Reduced cost = " + dj[j]);
            }

            int ncons = slack.Length;
            for (int i = 0; i < ncons; ++i) {
               System.Console.WriteLine("Constraint " + i +
                                        ": Slack = " + slack[i] +
                                        " Pi = " + pi[i]);
            }

            cplex.ExportModel("qpex1.lp");
         }
         cplex.End();
      }
      catch (ILOG.Concert.Exception e) {
         System.Console.WriteLine("Concert exception '" + e + "' caught");
      }
   }

   internal static ILPMatrix PopulateByRow(IMPModeler model) {
      ILPMatrix lp = model.AddLPMatrix();

      double[]    lb = {0.0, 0.0, 0.0};
      double[]    ub = {40.0, System.Double.MaxValue, System.Double.MaxValue};
      INumVar[]   x  = model.NumVarArray(model.ColumnArray(lp, 3), lb, ub);

      // - x0 +   x1 + x2 <= 20
      //   x0 - 3*x1 + x2 <= 30
      double[]   lhs = {-System.Double.MaxValue, -System.Double.MaxValue};
      double[]   rhs = {20.0, 30.0};
      double[][] val = { new double[] {-1.0,  1.0,  1.0},
                         new double[] { 1.0, -3.0,  1.0} };
      int[][]    ind = { new int[] {0, 1, 2},
                         new int[] {0, 1, 2} };
      lp.AddRows(lhs, rhs, ind, val);

      // Q = 0.5 ( 33*x0*x0 + 22*x1*x1 + 11*x2*x2 - 12*x0*x1 - 23*x1*x2 )
      INumExpr x00 = model.Prod( 33.0, model.Square(x[0]));
      INumExpr x11 = model.Prod( 22.0, model.Square(x[1]));
      INumExpr x22 = model.Prod( 11.0, model.Square(x[2]));
      INumExpr x01 = model.Prod(-12.0, model.Prod(x[0], x[1]));
      INumExpr x12 = model.Prod(-23.0, model.Prod(x[1], x[2]));
      INumExpr Q   = model.Prod(0.5, model.Sum(x00, x11, x22, x01, x12));

      // maximize x0 + 2*x1 + 3*x2 + Q
      double[] objvals = {1.0, 2.0, 3.0};
      model.Add(model.Maximize(model.Diff(model.ScalProd(x, objvals), Q)));

      return (lp);
   }
}
