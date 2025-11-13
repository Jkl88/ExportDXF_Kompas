using Kompas6Constants;
using Kompas6Constants3D;
using KompasAPI7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportDXF_Kompas
{
    public class PartInfo
    {
        private bool _assembly = false;
        private bool _selected = false;
        private bool _standart = false;
        private string _name;
        private string _marking;
        private string _filePath;
        private string _type = "Неизвестный тип";
        private bool _hasUnfold = false;
        private bool _isSheet = false;
        private double _thickness;
        private int _bendCount;
        private string _view;
        private int _embodimentIndex;
        private string _owner;
        private int _count = 1;
        private List<string> _projections = new List<string>();
        private IPart7 _part = null;

        public bool Assembly
        {
            get => _assembly;
            set { _assembly = value; }
        }
        public bool Selected
        {
            get => _selected;
            set { _selected = value; }
        }

        public bool Standart
        {
            get => _standart;
            set { _standart = value; }
        }

        public string Name
        {
            get => _name;
            set { _name = value; }
        }

        public string Marking
        {
            get => _marking;
            set { _marking = value; }
        }

        public string FilePath
        {
            get => _filePath;
            set { _filePath = value; }
        }

        public string Type
        {
            get => _type;
            set { _type = value; }
        }

        public bool HasUnfold
        {
            get => _hasUnfold;
            set { _hasUnfold = value; }
        }

        public bool IsSheet
        {
            get => _isSheet;
            set { _isSheet = value; }
        }

        public double Thickness
        {
            get => _thickness;
            set { _thickness = value; }
        }

        public int BendCount
        {
            get => _bendCount;
            set { _bendCount = value; }
        }

        public string View
        {
            get => _view;
            set { _view = value; }
        }

        public int EmbodimentIndex
        {
            get => _embodimentIndex;
            set { _embodimentIndex = value; }
        }

        public string Owner
        {
            get => _owner;
            set { _owner = value; }
        }

        public int Count
        {
            get => _count;
            set { _count = value; }
        }

        public List<string> Projections
        {
            get => _projections;
            set { _projections = value ?? new List<string>(); }
        }

        public IPart7 Part
        {
            get => _part;
            set { _part = value ?? null; }
        }
    }
}
