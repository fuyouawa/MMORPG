[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(MMORPG.Tool.Editor.InformationValidator))]

namespace MMORPG.Tool.Editor
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.OdinInspector.Editor.ValueResolvers;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    [DrawerPriority(0.0, 10001.0, 0.0)]
    public class InformationAttributeDrawer : OdinAttributeDrawer<InformationAttribute>
    {
        private bool _drawMessageBox;
        private ValueResolver<bool> _visibleIfResolver;
        private ValueResolver<string> _messageResolver;
        private ValueResolver<Color> _iconColorResolver;
        private MessageType _messageType;

        protected override void Initialize()
        {
            _visibleIfResolver = ValueResolver.Get(base.Property, base.Attribute.VisibleIf, fallbackValue: true);
            _messageResolver = ValueResolver.GetForString(base.Property, base.Attribute.Message);
            _iconColorResolver = ValueResolver.Get<Color>(base.Property, base.Attribute.IconColor, EditorStyles.label.normal.textColor);
            _drawMessageBox = _visibleIfResolver.GetValue();
            _messageType = Attribute.MessageType switch
            {
                InfoMessageType.Info => MessageType.Info,
                InfoMessageType.Warning => MessageType.Warning,
                InfoMessageType.Error => MessageType.Error,
                _ => MessageType.None
            };
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            bool flag = true;
            if (_visibleIfResolver.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(_visibleIfResolver.ErrorMessage);
                flag = false;
            }
            if (_messageResolver.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(_messageResolver.ErrorMessage);
                flag = false;
            }
            if (_iconColorResolver.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(_iconColorResolver.ErrorMessage);
                flag = false;
            }
            if (!flag)
            {
                CallNextDrawer(label);
                return;
            }
            if (base.Attribute.GUIAlwaysEnabled)
            {
                GUIHelper.PushGUIEnabled(enabled: true);
            }
            if (UnityEngine.Event.current.type == EventType.Layout)
            {
                _drawMessageBox = _visibleIfResolver.GetValue();
            }
            if (_drawMessageBox)
            {
                string value = _messageResolver.GetValue();
                SirenixEditorGUI.MessageBox(value, _messageType, SirenixGUIStylesHelper.MessageBox, true);
            }
            if (base.Attribute.GUIAlwaysEnabled)
            {
                GUIHelper.PopGUIEnabled();
            }
            CallNextDrawer(label);
        }
    }

    [NoValidationInInspector]
    public class InformationValidator : AttributeValidator<InformationAttribute>
    {
        private ValueResolver<bool> _showMessageGetter;

        private ValueResolver<string> _messageGetter;

        protected override void Initialize()
        {
            _showMessageGetter = ValueResolver.Get(base.Property, base.Attribute.VisibleIf, fallbackValue: true);
            _messageGetter = ValueResolver.GetForString(base.Property, base.Attribute.Message);
        }

        protected override void Validate(ValidationResult result)
        {
            if (_showMessageGetter != null)
            {
                if (_showMessageGetter.HasError || _messageGetter.HasError)
                {
                    result.Message = ValueResolver.GetCombinedErrors(_showMessageGetter, _messageGetter);
                    result.ResultType = ValidationResultType.Error;
                }
                else if (_showMessageGetter.GetValue())
                {
                    result.ResultType = Attribute.MessageType switch
                    {
                        InfoMessageType.Warning => ValidationResultType.Warning,
                        InfoMessageType.Error => ValidationResultType.Error,
                        _ => ValidationResultType.Valid
                    };
                    result.Message = _messageGetter.GetValue();
                }
            }
        }
    }

}
