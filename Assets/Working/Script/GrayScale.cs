using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(GrayScaleRenderer), PostProcessEvent.AfterStack, "Custom/Grayscale_PP")]
public sealed class GrayScale : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Grayscale effect intensity")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
}

public sealed class GrayScaleRenderer : PostProcessEffectRenderer<GrayScale>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Grayscale_PP"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
