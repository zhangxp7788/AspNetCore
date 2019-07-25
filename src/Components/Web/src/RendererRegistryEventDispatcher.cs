// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Microsoft.AspNetCore.Components.Web
{
    /// <summary>
    /// Provides mechanisms for dispatching events to components in a <see cref="Renderer"/>.
    /// </summary>
    public static class RendererRegistryEventDispatcher
    {
        /// <summary>
        /// For framework use only.
        /// </summary>
        [JSInvokable(nameof(DispatchEvent))]
        public static Task DispatchEvent(BrowserEventDescriptor eventDescriptor, string eventArgsJson)
        {
            InterpretEventDescriptor(eventDescriptor);
            var eventArgs = ParseEventArgsJson(eventDescriptor.EventHandlerId, eventDescriptor.EventArgsType, eventArgsJson);
            var renderer = RendererRegistry.Current.Find(eventDescriptor.BrowserRendererId);
            return renderer.DispatchEventAsync(eventDescriptor.EventHandlerId, eventDescriptor.EventFieldInfo, eventArgs);
        }

        private static void InterpretEventDescriptor(BrowserEventDescriptor eventDescriptor)
        {
            // The incoming field value can be either a bool or a string, but since the .NET property
            // type is 'object', it will deserialize initially as a JsonElement
            var fieldInfo = eventDescriptor.EventFieldInfo;
            if (fieldInfo != null)
            {
                if (fieldInfo.FieldValue is JsonElement attributeValueJsonElement)
                {
                    switch (attributeValueJsonElement.ValueKind)
                    {
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            fieldInfo.FieldValue = attributeValueJsonElement.GetBoolean();
                            break;
                        default:
                            fieldInfo.FieldValue = attributeValueJsonElement.GetString();
                            break;
                    }
                }
                else
                {
                    // Unanticipated value type. Ensure we don't do anything with it.
                    eventDescriptor.EventFieldInfo = null;
                }
            }
        }

        private static UIEventArgs ParseEventArgsJson(ulong eventHandlerId, string eventArgsType, string eventArgsJson)
        {
            try
            {
                return eventArgsType switch
                {
                    "change" => DeserializeUIEventChangeArgs(eventArgsJson),
                    "clipboard" => Deserialize<UIClipboardEventArgs>(eventArgsJson),
                    "drag" => Deserialize<UIDragEventArgs>(eventArgsJson),
                    "error" => Deserialize<UIErrorEventArgs>(eventArgsJson),
                    "focus" => Deserialize<UIFocusEventArgs>(eventArgsJson),
                    "keyboard" => Deserialize<UIKeyboardEventArgs>(eventArgsJson),
                    "mouse" => Deserialize<UIMouseEventArgs>(eventArgsJson),
                    "pointer" => Deserialize<UIPointerEventArgs>(eventArgsJson),
                    "progress" => Deserialize<UIProgressEventArgs>(eventArgsJson),
                    "touch" => Deserialize<UITouchEventArgs>(eventArgsJson),
                    "unknown" => Deserialize<UIEventArgs>(eventArgsJson),
                    "wheel" => Deserialize<UIWheelEventArgs>(eventArgsJson),
                    _ => throw new InvalidEventException(eventHandlerId, $"Unsupported event type '{eventArgsType}'."),
                };
            }
            catch (Exception e)
            {
                throw new InvalidEventException(eventHandlerId, "There was an error parsing the event arguments.", e);
            }
        }

        private static T Deserialize<T>(string eventArgsJson)
        {
            return JsonSerializer.Deserialize<T>(eventArgsJson, JsonSerializerOptionsProvider.Options);
        }

        private static UIChangeEventArgs DeserializeUIEventChangeArgs(string eventArgsJson)
        {
            var changeArgs = Deserialize<UIChangeEventArgs>(eventArgsJson);
            var jsonElement = (JsonElement)changeArgs.Value;
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Null:
                    changeArgs.Value = null;
                    break;
                case JsonValueKind.String:
                    changeArgs.Value = jsonElement.GetString();
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    changeArgs.Value = jsonElement.GetBoolean();
                    break;
                default:
                    throw new ArgumentException($"Unsupported {nameof(UIChangeEventArgs)} value {jsonElement}.");
            }
            return changeArgs;
        }

        /// <summary>
        /// For framework use only.
        /// </summary>
        public class BrowserEventDescriptor
        {
            /// <summary>
            /// For framework use only.
            /// </summary>
            public int BrowserRendererId { get; set; }

            /// <summary>
            /// For framework use only.
            /// </summary>
            public ulong EventHandlerId { get; set; }

            /// <summary>
            /// For framework use only.
            /// </summary>
            public string EventArgsType { get; set; }

            /// <summary>
            /// For framework use only.
            /// </summary>
            public EventFieldInfo EventFieldInfo { get; set; }
        }
    }
}
