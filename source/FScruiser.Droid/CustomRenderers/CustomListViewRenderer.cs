using Android.Content;
using FScruiser.XF.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AListView = Android.Widget.ListView;

[assembly: ExportRenderer(typeof(global::FScruiser.XF.Views.CustomListView), typeof(global::FScruiser.Droid.CustomRenderers.CustomListViewRenderer))]

namespace FScruiser.Droid.CustomRenderers
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        private int _cachedFirstVisableItem;
        private bool _isScrolling;

        public CustomListViewRenderer(Context context) : base(context)
        {
        }

        //protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        //{
        //    base.OnScrollChanged(l, t, oldl, oldt);

        //    if (Element is CustomListView listViewController)
        //    {
        //        listViewController?.RaiseScroll();
        //    }
        //}

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if(e.OldElement != null)
            {
                AListView nativeListView = Control;
                if (nativeListView != null)
                {
                    nativeListView.Scroll -= NativeListView_Scroll;
                }
            }
            if (e.NewElement != null)
            {
                AListView nativeListView = Control;
                if(nativeListView != null)
                {
                    nativeListView.Scroll += NativeListView_Scroll;
                }
                _cachedFirstVisableItem = -1;
            }
        }


        private void NativeListView_Scroll(object sender, Android.Widget.AbsListView.ScrollEventArgs e)
        {
            if (Element is CustomListView listViewController)
            {
                if (_cachedFirstVisableItem != e.FirstVisibleItem && !_isScrolling)
                {
                    _isScrolling = true;
                    listViewController?.RaiseScroll();
                    _isScrolling = false;
                }

                _cachedFirstVisableItem = e.FirstVisibleItem;
            }
        }
    }
}