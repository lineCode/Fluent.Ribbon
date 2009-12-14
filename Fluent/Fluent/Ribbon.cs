﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    ///   Represents the main Ribbon control which consists of multiple tabs, each of which
    ///   containing groups of controls.  The Ribbon also provides improved context
    ///   menus, enhanced screen tips, and keyboard shortcuts.
    /// </summary>
    [ContentProperty("Tabs")]
    [TemplatePart(Name = "PART_BackstageButton", Type = typeof(BackstageButton))]
    public class Ribbon: Control
    {
        #region Fields

        // Collection of contextual tab groups
        private ObservableCollection<RibbonContextualTabGroup> groups;
        // Collection of tabs
        private ObservableCollection<RibbonTabItem> tabs;
        // Collection of toolbar items
        private ObservableCollection<UIElement> toolBarItems;
        // Collection of backstagetems
        private ObservableCollection<UIElement> backstageItems;
        // Ribbon title bar
        private RibbonTitleBar titleBar;
        // Ribbon tab control
        private RibbonTabControl tabControl;
        // Ribbon quick access toolbar
        private QuickAccessToolBar quickAccessToolBar;
        // Ribbon layout root
        private Panel layoutRoot;
        // Ribbon backstage button
        private BackstageButton backstageButton;

        // Handles F10, Alt and so on
        KeyTipService keyTipService;
        
        // Collection of quickaccess menu items
        private ObservableCollection<QuickAccessMenuItem> quickAccessItems;
        // Adornet for backstage
        private BackstageAdorner adorner;
        // Saved when backstage opened tab item
        private RibbonTabItem savedTabItem;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets KeyTip.Keys for Backstage
        /// </summary>
        public string BackstageKeyTipKeys
        {
            get { return (string)GetValue(BackstageKeyTipKeysProperty); }
            set { SetValue(BackstageKeyTipKeysProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BackstageKeyTipKeys. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackstageKeyTipKeysProperty =
            DependencyProperty.Register("BackstageKeyTipKeys", typeof(string), typeof(Ribbon), new UIPropertyMetadata(""));


        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        internal  RibbonTitleBar TitleBar
        {
            get { return titleBar; }
        }

        /// <summary>
        /// Gets or sets whether quick access toolbar showes above ribbon
        /// </summary>
        public bool ShowQuickAccessToolBarAboveRibbon
        {
            get { return (bool)GetValue(ShowQuickAccessToolBarAboveRibbonProperty); }
            set { SetValue(ShowQuickAccessToolBarAboveRibbonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAboveRibbon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowQuickAccessToolBarAboveRibbonProperty =
            DependencyProperty.Register("ShowQuickAccessToolBarAboveRibbon", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false, OnShowQuickAccesToolBarAboveRibbonChanged));
        
        /// <summary>
        /// Handles ShowQuickAccessToolBarAboveRibbon property changed
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnShowQuickAccesToolBarAboveRibbonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon ribbon = d as Ribbon;
            if (ribbon.titleBar != null) ribbon.titleBar.InvalidateMeasure();
        }

        /// <summary>
        /// Gets collection of contextual tab groups
        /// </summary>
        public ObservableCollection<RibbonContextualTabGroup> Groups
        {
            get
            {
                if (this.groups == null)
                {
                    this.groups = new ObservableCollection<RibbonContextualTabGroup>();
                    this.groups.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupsCollectionChanged);
                }
                return this.groups;
            }
        }
        /// <summary>
        /// Handles collection of contextual tab groups ghanges
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {                        
                        if (titleBar != null) titleBar.Items.Add(obj2);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {                        
                        if (titleBar != null) titleBar.Items.Remove(obj3);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {                        
                        if (titleBar != null) titleBar.Items.Remove(obj4);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (titleBar != null) titleBar.Items.Add(obj5);
                    }
                    break;
            }

        }

        /// <summary>
        /// gets collection of ribbon tabs
        /// </summary>
        public ObservableCollection<RibbonTabItem> Tabs
        {
            get
            {
                if (this.tabs == null)
                {
                    this.tabs = new ObservableCollection<RibbonTabItem>();
                    this.tabs.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnTabsCollectionChanged);
                }
                return this.tabs;
            }
        }

        /// <summary>
        /// Handles collection of ribbon tabs changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnTabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.Items.Add(obj2);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.Items.Remove(obj3);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.Items.Remove(obj4);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.Items.Add(obj5);
                    }
                    break;
            }

        }
        /// <summary>
        /// Gets collection of toolbar items
        /// </summary>
        public ObservableCollection<UIElement> ToolBarItems
        {
            get
            {
                if (this.toolBarItems == null)
                {
                    this.toolBarItems = new ObservableCollection<UIElement>();
                    this.toolBarItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnToolbarItemsCollectionChanged);
                }
                return this.toolBarItems;
            }
        }

        /// <summary>
        /// handles colection of toolbar i8tenms changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Add(obj2 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Add(obj5 as UIElement);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets quick access toolbar associated with the ribbon
        /// </summary>
        internal QuickAccessToolBar QuickAccessToolBar
        {
            get { return quickAccessToolBar; }
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                if(layoutRoot!=null)list.Add(layoutRoot);
                if(ShowQuickAccessToolBarAboveRibbon)if (quickAccessToolBar != null) list.Add(quickAccessToolBar);
                return list.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets collectionof quick access menu items
        /// </summary>
        public ObservableCollection<QuickAccessMenuItem> QuickAccessItems
        {
            get
            {
                if (this.quickAccessItems == null)
                {
                    this.quickAccessItems = new ObservableCollection<QuickAccessMenuItem>();
                    this.quickAccessItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnQuickAccessItemsCollectionChanged);
                }
                return this.quickAccessItems;
            }
        }
        /// <summary>
        /// Handles collectionof quick access menu items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnQuickAccessItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Add(obj2 as QuickAccessMenuItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Remove(obj3 as QuickAccessMenuItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Remove(obj4 as QuickAccessMenuItem);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Add(obj5 as QuickAccessMenuItem);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets collection of backstage items
        /// </summary>
        public ObservableCollection<UIElement> BackstageItems
        {
            get
            {
                if (this.backstageItems == null)
                {
                    this.backstageItems = new ObservableCollection<UIElement>();
                    this.backstageItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnBackstageItemsCollectionChanged);
                }
                return this.backstageItems;
            }
        }

        /// <summary>
        /// Handles collection of backstage items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Th event data</param>
        private void OnBackstageItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Add(obj2 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Add(obj5 as UIElement);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets or sets whether backstage is opened
        /// </summary>
        public bool IsBackstageOpen
        {
            get { return (bool)GetValue(IsBackstageOpenProperty); }
            set { SetValue(IsBackstageOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsBackstageOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsBackstageOpenProperty =
            DependencyProperty.Register("IsBackstageOpen", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false, OnIsBackstageOpenChanged));

        /// <summary>
        /// Handles IsBackstageOpen property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnIsBackstageOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon ribbon = d as Ribbon;
            if((bool)e.NewValue)
            {
                ribbon.ShowBackstage();
            }
            else
            {
                ribbon.HideBackstage();
            }
        }

        /// <summary>
        /// Gets or sets backstage brush
        /// </summary>
        public Brush BackstageBrush
        {
            get { return (Brush)GetValue(BackstageBrushProperty); }
            set { SetValue(BackstageBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BackstageBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackstageBrushProperty =
            DependencyProperty.Register("BackstageBrush", typeof(Brush), typeof(Ribbon), new UIPropertyMetadata(Brushes.Blue));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static Ribbon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(typeof(Ribbon)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ribbon()
        {
            keyTipService = new KeyTipService(this);
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);
            Focusable = false;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion        

        #region Overrides

        /// <summary>
        /// Invoked whenever an unhandled System.Windows.UIElement.GotFocus event reaches this element in its route.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (tabControl != null) (tabControl.SelectedItem as RibbonTabItem).Focus();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (layoutRoot!=null) RemoveLogicalChild(layoutRoot);
            layoutRoot = GetTemplateChild("PART_LayoutRoot") as Panel;
            if (layoutRoot != null) AddLogicalChild(layoutRoot);
            
            if(titleBar!=null)
            {
                titleBar.Items.Clear();
            }
            titleBar = GetTemplateChild("PART_RibbonTitleBar") as RibbonTitleBar;
            if((titleBar!=null)&&(groups!=null))
            {
                for(int i=0;i<groups.Count;i++)
                {
                    titleBar.Items.Add(groups[i]);
                }
            }

            if (tabControl != null)
            {
                tabControl.Items.Clear();
                tabControl.ToolBarItems.Clear();
                tabControl.PreviewMouseRightButtonUp -= OnTabControlRightButtonUp;
                tabControl.SelectionChanged -= OnTabControlSelectionChanged;
            }
            tabControl = GetTemplateChild("PART_RibbonTabControl") as RibbonTabControl;
            if (tabControl != null)
            {
                tabControl.PreviewMouseRightButtonUp += OnTabControlRightButtonUp;
                tabControl.SelectionChanged += OnTabControlSelectionChanged;
            }
            if ((tabControl != null)&&(tabs!=null))
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabControl.Items.Add(tabs[i]);
                }
            }

            if ((tabControl != null) && (toolBarItems != null))
            {
                for (int i = 0; i < toolBarItems.Count; i++)
                {
                    tabControl.ToolBarItems.Add(toolBarItems[i]);
                }
            }

            if (quickAccessToolBar != null)
            {
                quickAccessToolBar.QuickAccessItems.Clear();
            }
            quickAccessToolBar = GetTemplateChild("PART_QuickAccessToolBar") as QuickAccessToolBar;
            if ((quickAccessToolBar != null) && (quickAccessItems != null))
            {
                for (int i = 0; i < quickAccessItems.Count; i++)
                {
                    quickAccessToolBar.QuickAccessItems.Add(quickAccessItems[i]);
                }
            }
            if (backstageButton != null)
            {
                backstageButton.Backstage.Items.Clear();
            }
            backstageButton = GetTemplateChild("PART_BackstageButton") as BackstageButton;
            if (backstageButton != null) 
            {
                Binding binding = new Binding("IsBackstageOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                backstageButton.SetBinding(BackstageButton.IsOpenProperty, binding);
                if (backstageItems != null)
                {
                    for (int i = 0; i < backstageItems.Count; i++)
                    {
                        backstageButton.Backstage.Items.Add(backstageItems[i]);
                    }
                }
            }

        }

        #endregion

        #region Event handling

        // Handles tab control selection chaged
        private void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (IsBackstageOpen)
                {
                    savedTabItem = e.AddedItems[0] as RibbonTabItem;
                    IsBackstageOpen = false;
                }
            }
        }

        // handles tab control right button click
        private void OnTabControlRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (quickAccessToolBar != null)
            {
                RibbonTabControl ribbonTabControl = sender as RibbonTabControl;
                UIElement control = QuickAccessItemsProvider.PickQuickAccessItem(ribbonTabControl,
                                                                                 e.GetPosition(ribbonTabControl));
                if (control != null)
                {                    
                    if (quickAccessToolBar.Items.Contains(control)) quickAccessToolBar.Items.Remove(control);
                    else quickAccessToolBar.Items.Add(control);
                    quickAccessToolBar.InvalidateMeasure();
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            keyTipService.Attach();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            keyTipService.Detach();
        }

        #endregion

        #region Private methods

        // Show backstage
        private void ShowBackstage()
        {            
            AdornerLayer layer = GetAdornerLayer(this);
            if (adorner == null)
            {                
                UIElement topLevelElement = Window.GetWindow(this).Content as UIElement;
                double topOffset=backstageButton.TranslatePoint(new Point(0, backstageButton.ActualHeight), topLevelElement).Y;
                adorner = new BackstageAdorner(topLevelElement, backstageButton.Backstage, topOffset);
            }            
            layer.Add(adorner);
            if (tabControl != null)
            {
                savedTabItem = tabControl.SelectedItem as RibbonTabItem;
                tabControl.SelectedItem = null;
            }
            if(quickAccessToolBar!=null) quickAccessToolBar.IsEnabled = false;
            if(titleBar!=null) titleBar.IsEnabled = false;
            Window.GetWindow(this).PreviewKeyDown += OnBackstageEscapeKeyDown;
        }        

        // hide backstage
        private void HideBackstage()
        {
            AdornerLayer layer = GetAdornerLayer(this);
            layer.Remove(adorner);
            if (tabControl != null) tabControl.SelectedItem = savedTabItem;
            if (quickAccessToolBar != null) quickAccessToolBar.IsEnabled = true;
            if (titleBar != null) titleBar.IsEnabled = true;
            Window.GetWindow(this).PreviewKeyDown -= OnBackstageEscapeKeyDown;
        }

        // Handles backstage Esc key keydown
        private void OnBackstageEscapeKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Escape)IsBackstageOpen = false;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Get adorner layer for element
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Adorner layer</returns>
        static AdornerLayer GetAdornerLayer(UIElement element)
        {
            UIElement current = element;
            while (true)
            {
                current = (UIElement)VisualTreeHelper.GetParent(current);
                if (current is AdornerDecorator) return AdornerLayer.GetAdornerLayer((UIElement)VisualTreeHelper.GetChild(current, 0));
            }
        }

        #endregion
    }
}
