using CruiseDAL.DataObjects;
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

            _fieldNameMapControl.Add(nameof(Tree.TreeNumber), _treeNumberField);
            _fieldNameMapControl.Add(nameof(Tree.SampleGroup), _sampleGroupField);
            _fieldNameMapControl.Add(nameof(Tree.Species), _speciesField);

            _fieldNameMapControl.Add(nameof(Tree.Aspect), _aspectField);
            _fieldNameMapControl.Add(nameof(Tree.ClearFace), _clearFaceField);
            _fieldNameMapControl.Add(nameof(Tree.CountOrMeasure), _countOrMeasureField);
            _fieldNameMapControl.Add(nameof(Tree.CrownRatio), _crownRatioField);
            _fieldNameMapControl.Add(nameof(Tree.DBH), _dbhField);
            _fieldNameMapControl.Add(nameof(Tree.DBHDoubleBarkThickness), _dbhDoubleBarkThicknessField);
            _fieldNameMapControl.Add(nameof(Tree.DefectCode), _defectCodeField);
            _fieldNameMapControl.Add(nameof(Tree.DiameterAtDefect), _diameterAtDefectField);
            _fieldNameMapControl.Add(nameof(Tree.DRC), _drcField);
            _fieldNameMapControl.Add(nameof(Tree.FormClass), _formClassField);
            _fieldNameMapControl.Add(nameof(Tree.Grade), _gradeField);
            _fieldNameMapControl.Add(nameof(Tree.HeightToFirstLiveLimb), _heightFllField);
            _fieldNameMapControl.Add(nameof(Tree.HiddenPrimary), _hiddenPrimaryField);
            _fieldNameMapControl.Add(nameof(Tree.IsFallBuckScale), _isFallBuckScaleField);
            _fieldNameMapControl.Add(nameof(Tree.KPI), _kpiField);
            _fieldNameMapControl.Add(nameof(Tree.LiveDead), _liveDeadField);
            _fieldNameMapControl.Add(nameof(Tree.MerchHeightPrimary), _merchHeightPrimaryField);
            _fieldNameMapControl.Add(nameof(Tree.MerchHeightSecondary), _merchHeightSecondaryField);
            //_fieldNameMapControl.Add(nameof(Tree.PoleLength), _poleLengthField);
            _fieldNameMapControl.Add(nameof(Tree.RecoverablePrimary), _recoverablePrimaryField);
            _fieldNameMapControl.Add(nameof(Tree.Remarks), _remarksField);
            _fieldNameMapControl.Add(nameof(Tree.SeenDefectPrimary), _seenDefectPrimaryField);
            _fieldNameMapControl.Add(nameof(Tree.SeenDefectSecondary), _seenDefectSecondaryField);
            _fieldNameMapControl.Add(nameof(Tree.Slope), _slopeField);
            _fieldNameMapControl.Add(nameof(Tree.STM), _isStmSwitch);
            _fieldNameMapControl.Add(nameof(Tree.TopDIBPrimary), _dibPrimaryField);
            _fieldNameMapControl.Add(nameof(Tree.TopDIBSecondary), _dibSecondaryField);
            _fieldNameMapControl.Add(nameof(Tree.TotalHeight), _totalHeightField);
            _fieldNameMapControl.Add(nameof(Tree.TreeCount), _treeCountField);
            _fieldNameMapControl.Add(nameof(Tree.UpperStemDiameter), _upperStemDiameterField);
            _fieldNameMapControl.Add(nameof(Tree.UpperStemHeight), _upperStemHeightField);
            _fieldNameMapControl.Add(nameof(Tree.VoidPercent), _voidPercentField);
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