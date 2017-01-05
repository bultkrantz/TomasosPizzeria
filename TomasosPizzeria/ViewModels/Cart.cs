using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.Cli.Utils;
using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class Cart
    {
        private List<CartLine> _lineCollection = new List<CartLine>();

        public virtual void AddItem(Matratt matratt, int quantity)
        {
            CartLine line = _lineCollection
                .Where(p => p.Matratt.MatrattId == matratt.MatrattId)
                .FirstOrDefault();

            if (line == null)
            {
                _lineCollection.Add(new CartLine
                {
                    Matratt = matratt,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public virtual void RemoveLine(Matratt matratt) =>
        _lineCollection.RemoveAll(l => l.Matratt.MatrattId == matratt.MatrattId);

        public virtual decimal ComputeTotalValue() => _lineCollection.Sum(e => e.Matratt.Pris* e.Quantity);
        public virtual void Clear() => _lineCollection.Clear();
        public virtual bool ContainsPizza() => _lineCollection.Exists(x => x.Matratt.MatrattTyp == 1);
        public virtual IEnumerable<CartLine> Lines => _lineCollection;

        public class CartLine
        {
            public int CartLineID { get; set; }
            public Matratt Matratt { get; set; }
            public int Quantity { get; set; }
        }
    }
}
