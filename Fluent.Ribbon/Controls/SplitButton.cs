﻿// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using Fluent.Extensibility;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents button control that allows
    /// you to add menu and handle clicks
    /// </summary>
    [TemplatePart(Name = "PART_Button", Type = typeof(ButtonBase))]
    public class SplitButton : DropDownButton, IToggleButton, ICommandSource, IKeyTipInformationProvider
    {
        #region Fields

        // Inner button
        private ToggleButton button;

        private SplitButton quickAccessButton;

        #endregion

        #region Properties

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                var baseEnumerator = base.LogicalChildren;
                while (baseEnumerator.MoveNext())
                {
                    yield return baseEnumerator.Current;
                }

                if (this.button != null)
                {
                    yield return this.button;
                }
            }
        }

        #region Command

        /// <inheritdoc />
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        [Bindable(true)]
        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }

            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        /// <inheritdoc />
        [Bindable(true)]
        [Localizability(LocalizationCategory.NeverLocalize)]
        [Category("Action")]
        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }

            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }

        /// <inheritdoc />
        [Bindable(true)]
        [Category("Action")]
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)this.GetValue(CommandTargetProperty);
            }

            set
            {
                this.SetValue(CommandTargetProperty, value);
            }
        }

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata());

        /// <summary>
        /// Identifies the routed Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata());

        /// <summary>
        /// Identifies the CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata());

        #endregion

        #region GroupName

        /// <inheritdoc />
        public string GroupName
        {
            get { return (string)this.GetValue(GroupNameProperty); }
            set { this.SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(SplitButton));

        #endregion

        #region IsChecked

        /// <inheritdoc />
        public bool? IsChecked
        {
            get { return (bool)this.GetValue(IsCheckedProperty); }
            set { this.SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty = System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnIsCheckedChanged, CoerceIsChecked));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (SplitButton)d;
            if (button.IsCheckable)
            {
                var nullable = (bool?)e.NewValue;
                if (nullable == null)
                {
                    button.RaiseEvent(new RoutedEventArgs(IndeterminateEvent, button));
                }
                else if (nullable.Value)
                {
                    button.RaiseEvent(new RoutedEventArgs(CheckedEvent, button));
                }
                else
                {
                    button.RaiseEvent(new RoutedEventArgs(UncheckedEvent, button));
                }
            }
        }

        private static object CoerceIsChecked(DependencyObject d, object basevalue)
        {
            var button = (SplitButton)d;

            if (button.IsCheckable == false)
            {
                return false;
            }

            return basevalue;
        }

        #endregion

        #region IsCheckable

        /// <summary>
        /// Gets or sets a value indicating whether SplitButton can be checked
        /// </summary>
        public bool IsCheckable
        {
            get { return (bool)this.GetValue(IsCheckableProperty); }
            set { this.SetValue(IsCheckableProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register(nameof(IsCheckable), typeof(bool), typeof(SplitButton), new PropertyMetadata(BooleanBoxes.FalseBox));

        #endregion

        #region DropDownToolTip

        /// <summary>
        /// Gets or sets tooltip of dropdown part of split button
        /// </summary>
        public object DropDownToolTip
        {
            get { return this.GetValue(DropDownToolTipProperty); }
            set { this.SetValue(DropDownToolTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DropDownToolTip.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownToolTipProperty =
            DependencyProperty.Register(nameof(DropDownToolTip), typeof(object), typeof(SplitButton), new PropertyMetadata());

        #endregion

        #region IsButtonEnabled

        /// <summary>
        /// Gets or sets a value indicating whether dropdown part of split button is enabled
        /// </summary>
        public bool IsButtonEnabled
        {
            get { return (bool)this.GetValue(IsButtonEnabledProperty); }
            set { this.SetValue(IsButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDropDownEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsButtonEnabledProperty =
            DependencyProperty.Register(nameof(IsButtonEnabled), typeof(bool), typeof(SplitButton), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region IsDefinitive

        /// <summary>
        /// Gets or sets whether ribbon control click must close backstage
        /// </summary>
        public bool IsDefinitive
        {
            get { return (bool)this.GetValue(IsDefinitiveProperty); }
            set { this.SetValue(IsDefinitiveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDefinitive.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDefinitiveProperty =
            DependencyProperty.Register(nameof(IsDefinitive), typeof(bool), typeof(SplitButton), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when user clicks
        /// </summary>
        public static readonly RoutedEvent ClickEvent = ButtonBase.ClickEvent.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Occurs when user clicks
        /// </summary>
        public event RoutedEventHandler Click
        {
            add
            {
                this.AddHandler(ClickEvent, value);
            }

            remove
            {
                this.RemoveHandler(ClickEvent, value);
            }
        }

        /// <summary>
        /// Occurs when button is checked
        /// </summary>
        public static readonly RoutedEvent CheckedEvent = System.Windows.Controls.Primitives.ToggleButton.CheckedEvent.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Occurs when button is checked
        /// </summary>
        public event RoutedEventHandler Checked
        {
            add
            {
                this.AddHandler(CheckedEvent, value);
            }

            remove
            {
                this.RemoveHandler(CheckedEvent, value);
            }
        }

        /// <summary>
        /// Occurs when button is unchecked
        /// </summary>
        public static readonly RoutedEvent UncheckedEvent = System.Windows.Controls.Primitives.ToggleButton.UncheckedEvent.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Occurs when button is unchecked
        /// </summary>
        public event RoutedEventHandler Unchecked
        {
            add
            {
                this.AddHandler(UncheckedEvent, value);
            }

            remove
            {
                this.RemoveHandler(UncheckedEvent, value);
            }
        }

        /// <summary>
        /// Occurs when button is unchecked
        /// </summary>
        public static readonly RoutedEvent IndeterminateEvent = System.Windows.Controls.Primitives.ToggleButton.IndeterminateEvent.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Occurs when button is unchecked
        /// </summary>
        public event RoutedEventHandler Indeterminate
        {
            add
            {
                this.AddHandler(IndeterminateEvent, value);
            }

            remove
            {
                this.RemoveHandler(IndeterminateEvent, value);
            }
        }

        #endregion

        #region Constructors

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
            FocusVisualStyleProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata());
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SplitButton()
        {
            ContextMenuService.Coerce(this);
            this.Click += this.OnClick;
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeEvents();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            // Always unsubscribe events to ensure we don't subscribe twice
            this.UnSubscribeEvents();

            if (this.button != null)
            {
                this.button.Click += this.OnButtonClick;
            }
        }

        private void UnSubscribeEvents()
        {
            if (this.button != null)
            {
                this.button.Click -= this.OnButtonClick;
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(e.OriginalSource, this) == false
                && ReferenceEquals(e.OriginalSource, this.quickAccessButton) == false)
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            this.UnSubscribeEvents();

            this.button = this.GetTemplateChild("PART_Button") as ToggleButton;

            base.OnApplyTemplate();

            this.SubscribeEvents();
        }

        /// <inheritdoc />
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!PopupService.IsMousePhysicallyOver(this.button))
            {
                base.OnPreviewMouseLeftButtonDown(e);
            }
            else
            {
                this.IsDropDownOpen = false;
            }
        }

        #region Overrides of DropDownButton

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter)
            {
                this.button.InvokeClick();
            }
        }

        #endregion

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }

        #endregion

        #region Quick Access Item Creating

        /// <inheritdoc />
        public override FrameworkElement CreateQuickAccessItem()
        {
            var buttonForQAT = new SplitButton
                               {
                                   CanAddButtonToQuickAccessToolBar = false
                               };

            buttonForQAT.Click += (sender, e) => this.RaiseEvent(e);
            buttonForQAT.DropDownOpened += this.OnQuickAccessOpened;

            RibbonProperties.SetSize(buttonForQAT, RibbonControlSize.Small);
            this.BindQuickAccessItem(buttonForQAT);
            this.BindQuickAccessItemDropDownEvents(buttonForQAT);
            this.quickAccessButton = buttonForQAT;
            return buttonForQAT;
        }

        /// <inheritdoc />
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.Bind(this, element, nameof(this.DisplayMemberPath), DisplayMemberPathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.GroupStyleSelector), GroupStyleSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.ItemContainerStyle), ItemContainerStyleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.ItemsPanel), ItemsPanelProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.ItemStringFormat), ItemStringFormatProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.ItemTemplate), ItemTemplateProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.IsChecked), IsCheckedProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, nameof(this.DropDownToolTip), DropDownToolTipProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, nameof(this.IsCheckable), IsCheckableProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.IsButtonEnabled), IsButtonEnabledProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.ContextMenu), ContextMenuProperty, BindingMode.Default);

            RibbonControl.Bind(this, element, nameof(this.ResizeMode), ResizeModeProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.HasTriangle), HasTriangleProperty, BindingMode.Default);

            RibbonControl.BindQuickAccessItem(this, element);
        }

        /// <summary>
        /// Gets or sets whether button can be added to quick access toolbar
        /// </summary>
        public bool CanAddButtonToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddButtonToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddButtonToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddButtonToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddButtonToQuickAccessToolBarProperty = DependencyProperty.Register(nameof(CanAddButtonToQuickAccessToolBar), typeof(bool), typeof(SplitButton), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

        #region Implementation of IKeyTipInformationProvider

        /// <inheritdoc />
        public IEnumerable<KeyTipInformation> GetKeyTipInformations(bool hide)
        {
            yield return new KeyTipInformation(this.KeyTip + "A", this.button, hide)
                {
                    VisualTarget = this
                };
            yield return new KeyTipInformation(this.KeyTip + "B", this, hide);
        }

        #endregion

        #endregion
    }
}
