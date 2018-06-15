using CruiseDAL.DataObjects;
using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Util
{
    public class TreeEditControlFactory
    {
        public static View MakeEditView(TreeFieldSetupDO field)
        {
            View editView = null;
            switch (field.Field)
            {
                case nameof(Tree.Stratum):
                    {
                        editView = MakeStratumPicker();
                        break;
                    }
                case nameof(Tree.SampleGroup):
                    {
                        editView = MakeSampleGroupPicker();
                        break;
                    }
                case nameof(Tree.Species):
                    {
                        editView = MakeSpeciesPicker();
                        break;
                    }
                case nameof(Tree.CountOrMeasure):
                    {
                        editView = MakeCountMeasurePicker();
                        break;
                    }
                case nameof(Tree.Aspect):
                case nameof(Tree.CrownRatio):
                case nameof(Tree.DBH):
                case nameof(Tree.DBHDoubleBarkThickness):
                //case nameof(Tree.Diameter):
                case nameof(Tree.DiameterAtDefect):
                case nameof(Tree.DRC):
                case nameof(Tree.FormClass):
                //case nameof(Tree.Height):
                case nameof(Tree.HeightToFirstLiveLimb):
                case nameof(Tree.KPI)://int
                case nameof(Tree.MerchHeightPrimary):
                case nameof(Tree.MerchHeightSecondary):
                case nameof(Tree.PoleLength):
                case nameof(Tree.RecoverablePrimary):
                case nameof(Tree.SeenDefectPrimary):
                case nameof(Tree.SeenDefectSecondary):
                case nameof(Tree.Slope):
                case nameof(Tree.TopDIBPrimary):
                case nameof(Tree.TopDIBSecondary):
                case nameof(Tree.TotalHeight):
                case nameof(Tree.TreeCount):
                case nameof(Tree.TreeNumber):
                case nameof(Tree.UpperStemDiameter):
                case nameof(Tree.UpperStemHeight):
                case nameof(Tree.VoidPercent):
                    {
                        editView = new Entry();
                        ((InputView)editView).Keyboard = Keyboard.Numeric;
                        ((Entry)editView).Behaviors.Add(new Xamarin.Toolkit.Behaviors.NumericValidationBehavior { TextColorInvalid = Color.Red });
                        Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
                        editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
                        break;
                    }
                case nameof(Tree.DefectCode):
                    {
                        editView = new Entry();
                        ((InputView)editView).Keyboard = Keyboard.Default;
                        Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
                        editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
                        break;
                    }
                default:
                    {
                        editView = new Entry();
                        Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
                        editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
                        break;
                    }
            }

            if(editView is Entry entry)
            {
                entry.Effects.Add(new Xamarin.Toolkit.Effects.EntrySelectAllText());
            }

            return editView;
        }

        public static View MakeStratumPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.Strata));
            view.SetBinding(Picker.SelectedItemProperty, nameof(TreeEditViewModel.Stratum));
            view.ItemDisplayBinding = new Binding(nameof(Stratum.Code));

            return view;
        }

        public static Picker MakeCountMeasurePicker()
        {
            var picker = new Picker();
            picker.Items.Add(string.Empty);
            picker.Items.Add("C");
            picker.Items.Add("M");
            picker.SetBinding(Picker.SelectedItemProperty, $"Tree.{nameof(Tree.CountOrMeasure)}");

            return picker;
        }

        public static Picker MakeSampleGroupPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.SampleGroups));
            view.SetBinding(Picker.SelectedItemProperty, nameof(TreeEditViewModel.SampleGroup));
            view.ItemDisplayBinding = new Binding(nameof(SampleGroup.Code));

            return view;
        }

        public static Picker MakeSpeciesPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.TreeDefaults));
            view.SetBinding(Picker.SelectedItemProperty, nameof(TreeEditViewModel.TreeDefaultValue));
            view.ItemDisplayBinding = new Binding(nameof(TreeDefaultValueDO.Species));

            return view;
        }
    }
}
