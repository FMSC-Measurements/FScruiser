using CruiseDAL.Schema;
using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.XF.Pages
{
    public partial class TreeEditPage : ContentPage
    {
        private Dictionary<string, VisualElement> _fieldNameMapControl;

        public TreeEditPage()
        {
            InitializeComponent();

            MakeFieldMap();
        }

        private void MakeFieldMap()
        {
            _fieldNameMapControl = new Dictionary<string, VisualElement>();

            _fieldNameMapControl.Add(nameof(Tree_Ex.TreeNumber), _treeNumberField);
            _fieldNameMapControl.Add("SampleGroup", _sampleGroupField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.Species), _speciesField);

            _fieldNameMapControl.Add(nameof(Tree_Ex.Aspect), _aspectField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.ClearFace), _clearFaceField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.CountOrMeasure), _countOrMeasureField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.CrownRatio), _crownRatioField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.DBH), _dbhField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.DBHDoubleBarkThickness), _dbhDoubleBarkThicknessField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.DefectCode), _defectCodeField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.DiameterAtDefect), _diameterAtDefectField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.DRC), _drcField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.FormClass), _formClassField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.Grade), _gradeField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.HeightToFirstLiveLimb), _heightFllField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.HiddenPrimary), _hiddenPrimaryField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.IsFallBuckScale), _isFallBuckScaleField);
            //_fieldNameMapControl.Add(nameof(Tree_Ex.KPI), _kpiField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.LiveDead), _liveDeadField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.MerchHeightPrimary), _merchHeightPrimaryField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.MerchHeightSecondary), _merchHeightSecondaryField);
            //_fieldNameMapControl.Add(nameof(Tree_Ex.PoleLength), _poleLengthField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.RecoverablePrimary), _recoverablePrimaryField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.Remarks), _remarksField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.SeenDefectPrimary), _seenDefectPrimaryField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.SeenDefectSecondary), _seenDefectSecondaryField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.Slope), _slopeField);
            //_fieldNameMapControl.Add(nameof(Tree_Ex.STM), _isStmSwitch);
            _fieldNameMapControl.Add(nameof(Tree_Ex.TopDIBPrimary), _dibPrimaryField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.TopDIBSecondary), _dibSecondaryField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.TotalHeight), _totalHeightField);
            //_fieldNameMapControl.Add(nameof(Tree.TreeCount), _treeCountField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.UpperStemDiameter), _upperStemDiameterField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.UpperStemHeight), _upperStemHeightField);
            _fieldNameMapControl.Add(nameof(Tree_Ex.VoidPercent), _voidPercentField);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateTreeFieldVisability();
        }

        private void UpdateTreeFieldVisability()
        {
            var viewModel = (TreeEditViewModel)BindingContext;

            foreach (var view in _fieldNameMapControl.Values)
            { view.IsVisible = false; }

            foreach(var field in viewModel.TreeFields)
            {
                if (_fieldNameMapControl.TryGetValue(field.Field, out VisualElement view))
                {
                    view.IsVisible = true;
                }
            }
        }
    }
}