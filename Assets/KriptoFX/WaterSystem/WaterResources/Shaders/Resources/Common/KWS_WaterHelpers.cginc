#define UNITY_PI            3.14159265359f

float4 SampleBilinearSmooth(Texture2D tex, SamplerState state, float2 uv, float4 texelSize)
{
	float2 textureResolution = texelSize.zw;
	uv = uv * textureResolution + 0.5;
	float2 iuv = floor(uv);
	float2 fuv = frac(uv);
	uv = iuv + fuv * fuv * (3.0 - 2.0 * fuv);
	uv = (uv - 0.5) / textureResolution;
	return tex.Sample(state, uv);
}

float4 SampleBilinearSmoothLevel(Texture2D tex, SamplerState state, float2 uv, float4 texelSize)
{
	float2 textureResolution = texelSize.zw;
	uv = uv * textureResolution + 0.5;
	float2 iuv = floor(uv);
	float2 fuv = frac(uv);
	uv = iuv + fuv * fuv * (3.0 - 2.0 * fuv);
	uv = (uv - 0.5) / textureResolution;
	return tex.SampleLevel(state, uv, 0);
}



inline half4 SampleSupersampledMip(sampler2D tex, float2 uv, float2 texSize, float bias)
{
	float2 dx = ddx(uv);
	float2 dy = ddy(uv);// manually calculate the per axis mip level, clamp to 0 to 1
	// and use that to scale down the derivatives
	dx *= saturate(
		0.5 * log2(dot(dx * texSize, dx * texSize))
	);
	dy *= saturate(
		0.5 * log2(dot(dy * texSize, dy * texSize))
	);// rotated grid uv offsets
	float2 uvOffsets = float2(0.125, 0.375);
	float4 offsetUV = float4(0.0, 0.0, 0.0, bias);// supersampled using 2x2 rotated grid

	half4 color;
	offsetUV.xy = uv + uvOffsets.x * dx + uvOffsets.y * dy;
	color = tex2Dbias(tex, offsetUV);
	offsetUV.xy = uv - uvOffsets.x * dx - uvOffsets.y * dy;
	color += tex2Dbias(tex, offsetUV);
	offsetUV.xy = uv + uvOffsets.y * dx - uvOffsets.x * dy;
	color += tex2Dbias(tex, offsetUV);
	offsetUV.xy = uv - uvOffsets.y * dx + uvOffsets.x * dy;
	color += tex2Dbias(tex, offsetUV);
	color *= 0.25;
	return color;
}

inline half4 SampleSupersampled(sampler2D tex, float2 uv, float2 texelSize, float bias)
{
	half4 color;
	float3 texelOffset = float3(texelSize.xy, 0);
	color = tex2Dbias(tex, float4(uv + texelOffset.xz, 0, bias));
	color += tex2Dbias(tex, float4(uv - texelOffset.xz, 0, bias));
	color += tex2Dbias(tex, float4(uv + texelOffset.zy, 0, bias));
	color += tex2Dbias(tex, float4(uv - texelOffset.zy, 0, bias));
	color *= 0.25;

	//color = color * 0.5 + tex2Dbias(tex, float4(uv, 0, bias)) * 0.5;

	return color;
}

inline half4 SampleSupersampledLod(sampler2D tex, float2 uv, float2 texelSize)
{
	half4 color;
	float3 texelOffset = float3(texelSize.xy, 0);
	color = tex2Dlod(tex, float4(uv + texelOffset.xz, 0, 0));
	color += tex2Dlod(tex, float4(uv - texelOffset.xz, 0, 0));
	color += tex2Dlod(tex, float4(uv + texelOffset.zy, 0, 0));
	color += tex2Dlod(tex, float4(uv - texelOffset.zy, 0, 0));
	color *= 0.25;
	return color;
}

inline half Pow3(half x)
{
	return x * x * x;
}

inline half ComputeSSS(half lh, float3 normalLod, half3 lightDir)
{
	return saturate(1 - lh) * max(0, dot(normalLod, -lightDir * float3(1, 0.1, 1)));
}

inline half3 ComputeDerivativeNormal(float3 pos)
{
	return normalize(cross(ddx(pos), ddy(pos) * _ProjectionParams.x));
}


inline half3 ComputeDisplaceUsingFlowMap(Texture2D displaceTex, SamplerState state, float2 flowMap, half3 displace, float2 uv, float time)
{
	half blendMask = abs(flowMap.x) + abs(flowMap.y);
	if (blendMask < 0.01) return displace;
	
	half time1 = frac(time + 0.5);
	half time2 = frac(time);
	half flowLerp = abs((0.5 - time1) / 0.5);
	half flowLerpFix = lerp(1, 0.65, abs(flowLerp * 2 - 1));

	half3 tex1 = displaceTex.SampleLevel(state, uv - 0.25 * flowMap * time1, 0).xyz;
	half3 tex2 = displaceTex.SampleLevel(state, uv - 0.25 * flowMap * time2, 0, 0).xyz;
	half3 flowDisplace = lerp(tex1, tex2, flowLerp);
	flowDisplace.xz *= flowLerpFix;
	return lerp(displace, flowDisplace, saturate(blendMask));
}


inline half3 ComputeNormalUsingFlowMap(Texture2D normTex, SamplerState state, float2 flowMap, half3 normal, float2 uv, float time)
{
	half blendMask = abs(flowMap.x) + abs(flowMap.y);
	//if (blendMask < 0.01) return normal;

	half time1 = frac(time + 0.5);
	half time2 = frac(time);
	half flowLerp = abs((0.5 - time1) / 0.5);
	half flowLerpFix = lerp(1, 0.65, abs(flowLerp * 2 - 1)); //fix interpolation bug, TODO: I need find what cause this.
	
	half3 tex1 = normTex.Sample(state, uv - 0.25 * flowMap * time1).xyz;
	half3 tex2 = normTex.Sample(state, uv - 0.25 * flowMap * time2).xyz;
	half3 flowNormal = lerp(tex1, tex2, flowLerp);
	flowNormal.xz *= flowLerpFix;
	return lerp(normal, flowNormal, saturate(blendMask));
}

inline void ComputeNormalsUsingDynamicRipples(sampler2D ripplesNormTex, sampler2D ripplesNormTexLast, sampler2D ripplesTex, float2 uv, inout half3 normal, inout half3 normalLod, inout float2 mainUV)
{
	half2 ripples = tex2D(ripplesTex, uv).xy;
	half2 ripplesPrev = tex2D(ripplesTex, uv).xy;
	ripples = lerp(ripples, ripplesPrev, 0.5).xy;
	half ripplesMask = tex2D(ripplesTex, uv).r;
	
	mainUV += ripples.xy / 10;
	//normal.xz += ripples.xy;
	
	normal.xz = lerp(normal.xz, ripples.xy, abs(ripplesMask));
	//normNotDetailed.xz += ripplesNormal * 2;
	normalLod.xz += ripples.xy;
	//return saturate(1 - saturate(abs(ripples.x * 5)));

}


inline half3 ViewNormal(half3 worldNorm)
{
	float3 viewNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
	return viewNorm;
}


inline half SelfSmithJointGGXVisibilityTerm(half NdotL, half NdotV, half roughness)
{
	half a = roughness;
	half lambdaV = NdotL * (NdotV * (1 - a) + a);
	half lambdaL = NdotV * (NdotL * (1 - a) + a);

	return 0.5f / (lambdaV + lambdaL + 1e-5f);
}


inline half SelfGGXTerm(half NdotH, half roughness)
{
	half a2 = roughness * roughness;
	half d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad
	return 0.31830988618f * a2 / (d * d + 1e-7f); // This function is not intended to be running on Mobile,
	// therefore epsilon is smaller than what can be represented by half
}


half2 Refract(half3 viewDir, half3 normal, float ior)
{
	half nv = dot(normal, viewDir);
	half v2 = dot(viewDir, viewDir);
	half knormal = (sqrt(((ior * ior - 1) * v2) / (nv * nv) + 1.0) - 1.0) * nv;
	
	return (knormal * normal).xz;
}

inline float3 ScreenToWorld(float2 UV, float depth, float4x4 projToView, float4x4 viewToWorld)
{
	float2 uvClip = UV * 2.0 - 1.0;
	float4 clipPos = float4(uvClip, depth, 1.0);
	float4 viewPos = mul(projToView, clipPos);
	viewPos /= viewPos.w;
	float3 worldPos = mul(viewToWorld, viewPos).xyz;
	return worldPos;
}

half3 KW_AmbientColor;

float Fresnel_IOR(float3 viewDir, float3 normal, float ior)
{
	float cosi = clamp(-1, 1, dot(viewDir, normal));
	float etai = 1, etat = ior;
	if (cosi > 0)
	{
		float temp = etat;
		etat = etai;
		etai = temp;
	}
	
	float sint = etai / etat * sqrt(max(0.f, 1 - cosi * cosi));
	
	if (sint >= 1)
	{
		return 1;
	}
	else
	{
		float cost = sqrt(max(0.f, 1 - sint * sint));
		cosi = abs(cosi);
		float Rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
		float Rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost));
		return (Rs * Rs + Rp * Rp) / 2;
	}
	// As a consequence of the conservation of energy, transmittance is given by:
	// kt = 1 - kr;

}

inline half ComputeSpecular(half nl, half nv, half nh, half viewDistNormalized, half smoothness)
{
	half V = SelfSmithJointGGXVisibilityTerm(nl, nv, smoothness);
	half D = SelfGGXTerm(nh, viewDistNormalized * 0.1 + smoothness);

	half specularTerm = V * D;
	//
	//#   ifdef UNITY_COLORSPACE_GAMMA
	//	specularTerm = sqrt(max(1e-4h, specularTerm));
	//#   endif


	specularTerm = max(0, specularTerm * nl * KWS_SunStrength);

	return specularTerm;
}

half3 ComputeUnderwaterColor(half3 refraction, half3 volumeLight, half fade, half transparent, half3 waterColor, half turbidity, half3 turbidityColor)
{
	fade = max(0, fade);
	float fadeExp = saturate(1 - exp(-5 * fade / transparent));
	half3 absorbedColor = pow(clamp(waterColor.xyz, 0.1, 0.95), 25 * fade / transparent); //min range ~ 0.0  with pow(x, 70)
	half3 waterColorInDepth = pow(waterColor.xyz, 15.0) * 0.05 * volumeLight.rgb;
	half3 turbidityColorInDepth = turbidityColor * volumeLight.rgb;
	
	absorbedColor = lerp(waterColorInDepth, refraction, absorbedColor);
	turbidityColor = lerp(refraction, turbidityColorInDepth, fadeExp);
	absorbedColor = lerp(absorbedColor, turbidityColor, turbidity);

	return absorbedColor;
}

half3 ComputeUnderwaterColor(half3 refraction, half3 volumeLight, half fade, half transparent, half3 waterColor, half turbidity, half3 turbidityColor, half3 fogOpacity, half3 fogColor)
{
	fade = max(0, fade);
	float fadeExp = saturate(1 - exp(-5 * fade / transparent));
	half3 absorbedColor = pow(clamp(waterColor.xyz, 0.1, 0.95), 25 * fade / transparent); //min range ~ 0.0  with pow(x, 70)
	half3 waterColorInDepth = lerp(pow(waterColor.xyz, 15.0) * 0.05 * volumeLight.rgb, fogColor, fogOpacity);
	half3 turbidityColorInDepth = lerp(turbidityColor * volumeLight.rgb, fogColor, fogOpacity);
	
	absorbedColor = lerp(waterColorInDepth, refraction, absorbedColor);
	turbidityColor = lerp(refraction, turbidityColorInDepth, fadeExp);
	absorbedColor = lerp(absorbedColor, turbidityColor, turbidity);

	return absorbedColor;
}

half3 ComputeUnderwaterSurfaceColor(half3 volumeLight, half transparent, half3 waterColor, half turbidity, half3 turbidityColor)
{
	half3 absorbedColor = pow(clamp(waterColor.xyz, 0.1, 0.95), 25); //min range ~ 0.0  with pow(x, 70)
	half3 waterColorInDepth = pow(waterColor.xyz, 15.0) * 0.05 * volumeLight.rgb;
	turbidityColor = turbidityColor * volumeLight.rgb;
	
	absorbedColor = lerp(waterColorInDepth, 1.0, absorbedColor);
	absorbedColor = lerp(absorbedColor, turbidityColor, turbidity);
	absorbedColor = lerp(absorbedColor, 0.5, saturate(transparent / 50.0));

	return absorbedColor;
}


float ComputeWaterFresnel(half3 normal, half3 viewDir)
{
	float x = 1 - saturate(dot(normal, viewDir));
	return 0.02 + 0.98 * x * x * x * x * x * x; //fresnel aproximation http://wiki.nuaj.net/images/thumb/1/16/Fresnel.jpg/800px-Fresnel.jpg

}

half3 ComputeSSS(float2 screenUV, half3 underwaterColor, half shadowMask, half KW_Transparent)
{
	half sssMask = GetWaterMaskScatterNormals(screenUV).y;
	float3 sssColor = dot(underwaterColor, 0.333) * 0.25 + underwaterColor * 0.75;
	return sssMask * shadowMask * sssColor * saturate(1 - KW_Transparent / 100);
}

half3 ComputeSunlight(half3 normal, half3 viewDir, float3 lightDir, float3 lightColor, half shadowMask, float viewDist, float waterFarDistance, half KW_Transparent)
{
	half3 halfDir = normalize(lightDir + viewDir);
	half nh = saturate(dot(normal, halfDir));
	half nl = saturate(dot(normal, lightDir));
	half lh = saturate(dot(lightDir, halfDir));
	half fresnel = saturate(dot(normal, viewDir));
	
	float viewDistNormalized = saturate(viewDist / (waterFarDistance * 2));
	half3 specular = ComputeSpecular(nl, fresnel, nh, viewDistNormalized, KWS_SunCloudiness);
	specular = clamp(specular * 10 - 2.5 * saturate(1 - KWS_SunCloudiness * 10), 0, KWS_SunMaxValue);
	//half sunset = saturate(0.01 + dot(lightDir, float3(0, 1, 0))) * 30;

	return shadowMask * specular * lightColor;
}


float2 GetScreenSpaceReflectionUV(float3 reflDir)
{
	reflDir.y = -reflDir.y;
	float4 projected = mul(UNITY_MATRIX_VP, float4(reflDir, 0));
	float2 uv = (projected.xy / projected.w) * 0.5f + 0.5f;
	
	#ifdef UNITY_UV_STARTS_AT_TOP
		uv.y = 1 - uv.y;
	#endif

	return uv;
}


inline half3 GetFilteredNormal_lod0(float2 uv, float viewDist, out float normalFilteringMask)
{
	half bicubicLodDist = 10 + (1 - KW_FFT_Size_Normalized) * 40;
	
	half3 bicubicNormal = Texture2DSampleBicubic(KW_NormTex, sampler_KW_NormTex, uv, KW_NormTex_TexelSize).xyz;
	half3 normalAA = Texture2DSampleAA(KW_NormTex, sampler_KW_NormTex, uv).xyz;
	half3 normal = lerp(bicubicNormal, normalAA, saturate(viewDist / bicubicLodDist)).xyz;

	float rlen = rcp(saturate(length(normal)));
	normalFilteringMask = rcp(1.0 + 100.0 * (rlen - 1.0));
	normalFilteringMask = lerp(1, normalFilteringMask, saturate(viewDist / 500));

	return normal;
}

inline half3 GetNormal_lod0(float2 uv)
{
	return KW_NormTex.Sample(sampler_KW_NormTex, uv).xyz;
}

inline half3 GetNormal_lod1_lod2(float3 worldPos, half3 normal)
{
	half3 norm_lod1 = KW_NormTex_LOD1.Sample(sampler_KW_NormTex, worldPos.xz / KW_FFTDomainSize_LOD1).xyz;
	half3 norm_lod2 = KW_NormTex_LOD2.Sample(sampler_KW_NormTex, worldPos.xz / KW_FFTDomainSize_LOD2).xyz;
	return KWS_BlendNormals(normal, norm_lod1, norm_lod2);
}

inline half3 GetFlowmapNormal(float3 worldPos, float2 uv, half3 normal)
{
	float2 flowMapUV = (worldPos.xz - KW_FlowMapOffset.xz) / KW_FlowMapSize + 0.5;
	float2 flowmap = KW_FlowMapTex.Sample(sampler_linear_clamp, flowMapUV).xy * 2 - 1;
	return ComputeNormalUsingFlowMap(KW_NormTex, sampler_linear_repeat, flowmap, normal, uv, _Time.y * KW_FlowMapSpeed);
}

inline half3 GetFluidsNormal(float3 worldPos, float2 uv, half3 normal, out half foam)
{
	float2 fluidsUV_lod0 = (worldPos.xz - KW_FluidsMapWorldPosition_lod0.xz) / KW_FluidsMapAreaSize_lod0 + 0.5;
	float4 fluidsData_lod0 = KW_Fluids_Lod0.SampleLevel(sampler_linear_clamp, fluidsUV_lod0, 0);

	float2 fluidsUV_lod1 = (worldPos.xz - KW_FluidsMapWorldPosition_lod1.xz) / KW_FluidsMapAreaSize_lod1 + 0.5;
	float4 fluidsData_lod1 = KW_Fluids_Lod1.SampleLevel(sampler_linear_clamp, fluidsUV_lod1, 0);

	float2 maskUV_lod0 = 1 - saturate(abs(fluidsUV_lod0 * 2 - 1));
	float lodLevelFluidMask_lod0 = saturate((maskUV_lod0.x * maskUV_lod0.y - 0.01) * 5);
	float2 maskUV_lod1 = 1 - saturate(abs(fluidsUV_lod1 * 2 - 1));
	float lodLevelFluidMask_lod1 = saturate((maskUV_lod1.x * maskUV_lod1.y - 0.01) * 5);

	float3 fluids = lerp(fluidsData_lod1.xyz, fluidsData_lod0.xyz, lodLevelFluidMask_lod0);
	fluids *= lodLevelFluidMask_lod1;

	normal = ComputeNormalUsingFlowMap(KW_NormTex, sampler_linear_repeat, fluids.xy * KW_FluidsVelocityAreaScale * 0.75, normal, uv, _Time.y * KW_FlowMapSpeed * 0.5);

	float foamMask_lod0 = KW_FluidsFoam_Lod0.SampleLevel(sampler_linear_clamp, fluidsUV_lod0, 0).x;
	float foamMask_lod1 = KW_FluidsFoam_Lod1.SampleLevel(sampler_linear_clamp, fluidsUV_lod1, 0).x;
	float foamTex_lod0 = KW_FluidsFoamTex.SampleBias(sampler_linear_repeat, worldPos.xz / 40 + fluidsData_lod0.xy * 0.125 + float2(-_Time.x * 1, 0), -3).x;
	float foamTex_lod1 = KW_FluidsFoamTex.SampleBias(sampler_linear_repeat, worldPos.xz / 100 + fluidsData_lod1.xy * 0.25 + float2(-_Time.x * 1, 0), -3).x;

	foamMask_lod1 = min(foamMask_lod1, (fluidsData_lod1.z - 0.5));
	half foamMask = lerp(foamMask_lod1, foamMask_lod0, lodLevelFluidMask_lod0);
	half foamTex = lerp(foamTex_lod1 * 1.5, foamTex_lod0, lodLevelFluidMask_lod0);
	foam = foamMask * foamTex * lodLevelFluidMask_lod1;

	return normal;
}

inline half3 GetFluidsColor(half3 underwaterColor, half4 volumeScattering, half fluidsFoam)
{
	float3 foamColor = clamp(GetAmbientColor() * 0.15 + volumeScattering.a * 0.0 + volumeScattering.rgb * 4, 0, 0.95);
	return lerp(underwaterColor, foamColor, fluidsFoam);
}

inline half4 GetFlowmapEditor(float3 worldPos, half3 normal)
{
	float2 flowMapUV = (worldPos.xz - KW_FlowMapOffset.xz) / KW_FlowMapSize + 0.5;
	if (flowMapUV.x < 0 || flowMapUV.x > 1 || flowMapUV.y < 0 || flowMapUV.y > 1) return float4(0.5, 0, 0, 1);
	return half4(pow((normal.xz + 0.75), 7), 1, 1);
}

inline half3 GetDynamicWaves(float3 worldPos, half3 normal)
{
	float2 dynamicWavesUV = (worldPos.xz - KW_DynamicWavesWorldPos.xz) / KW_DynamicWavesAreaSize + 0.5;
	half3 dynamicWavesNormals = KW_DynamicWavesNormal.Sample(sampler_linear_clamp, dynamicWavesUV).xyz * 2 - 1;
	dynamicWavesNormals = normalize(half3(dynamicWavesNormals.x * 0.35, 1, dynamicWavesNormals.y * 0.35));
	return KWS_BlendNormals(normal, dynamicWavesNormals);
}

float3 BoxProjection(float3 direction, float3 position, float3 cubemapPosition, float3 boxMin, float3 boxMax)
{
	float3 factors = ((direction > 0 ?      boxMax : boxMin) - position) / direction;
	float scalar = min(min(factors.x, factors.y), factors.z);
	direction = direction * scalar + (position - cubemapPosition);
	return direction;
}


inline half3 GetCubemapReflectionFiltered(float3 reflDir, half3 reflection)
{
	float cubemapReflectionVerticalOffset = lerp(0, 0.3, saturate((KW_WindSpeed - 0.5) * 0.2));
	half3 reflectionWithOffset = GetCubemapReflection(lerp(reflDir, float3(0, 1, 0), cubemapReflectionVerticalOffset));
	return lerp(reflectionWithOffset, reflection, saturate(dot(reflection - 1.0, 0.33)));
}


half3 ComputeWaterRefractRay(half3 viewDir, half3 normal, half depth)
{
	half nv = dot(normal, viewDir);
	half v2 = dot(viewDir, viewDir);
	half knormal = (sqrt(((1.7689 - 1.0) * v2) / (nv * nv) + 1.0) - 1.0) * nv;
	half3 result = depth * (viewDir + (knormal * normal));
	result.y = result.y * 0.35; //fix information lost in the near camera
	return result;
}



inline float2 GetRefractedUV_IOR(float3 viewDir, half3 normal, float3 worldPos, float surfaceTensionFade)
{
	float3 refractedRay = ComputeWaterRefractRay(-viewDir, normal, KWS_RefractionAproximatedDepth * surfaceTensionFade);
	float4 refractedClipPos = mul(UNITY_MATRIX_VP, float4(GetCameraRelativePosition(worldPos + refractedRay), 1.0));
	float4 refractionScreenPos = ComputeScreenPos(refractedClipPos);
	float2 uv = refractionScreenPos.xy / refractionScreenPos.w;
	return uv;
}

inline float2 GetRefractedUV_Simple(float2 uv, half3 normal)
{
	return uv + normal.xz * KWS_RefractionSimpleStrength * 0.5;
}

inline half GetSurfaceTension(float z, float screenPosW)
{
	return saturate((LinearEyeDepth(z) - screenPosW) * 8);
}

inline half GetWaterRawFade(float3 worldPos, float surfaceDepthZ, float refractedSceneZ, half surfaceMask, half depthAngleFix)
{
	return (LinearEyeDepth(refractedSceneZ) - surfaceDepthZ) * depthAngleFix;
}

inline void FixAboveWaterRendering(float2 refractionUV, float refractedSceneZ, float3 worldPos, float sceneZ, float surfaceDepthZ, float depthAngleFix, float2 screenUV, half surfaceMask, inout float fade, inout half3 refraction, inout half4 volumeScattering)
{
	float3 worldSpaceDepth = GetWorldSpacePositionFromDepth(refractionUV, refractedSceneZ);

	UNITY_BRANCH if (surfaceMask > 0.5 && worldSpaceDepth.y > worldPos.y + 0.25)
	{
		fade = (LinearEyeDepth(sceneZ) - surfaceDepthZ) * depthAngleFix;
		refraction = GetSceneColor(screenUV);
		#if USE_VOLUMETRIC_LIGHT
			volumeScattering = GetVolumetricLight(screenUV);
		#endif
	}
}

inline half3 ApplyShorelineWavesReflectionFix(float3 reflDir, half3 reflection, half3 underwaterColor)
{
	float r = 1 - saturate(dot(reflDir, float3(0, 1, 0)));
	return lerp(reflection, max(underwaterColor, reflection), Pow5(r));
}

inline bool IsOutsideUV(float2 uv)
{
	return uv.x < 0.001 || uv.x > 0.999 || uv.y < 0.001 || uv.y > 0.999;
}

float2 GetWaterOrthoDepthUV(float3 worldPos)
{
	return (worldPos.xz - KWS_OrthoDepthPos.xz) / KWS_OrthoDepthNearFarSize.z + 0.5;
}

float GetWaterOrthoDepth(float2 uv)
{
	float near = KWS_OrthoDepthNearFarSize.x;
	float far = KWS_OrthoDepthNearFarSize.y;
	float terrainDepth = KWS_WaterOrthoDepthRT.SampleLevel(sampler_linear_clamp, uv, 0).r * (far - near) - far;
	return terrainDepth;
}

inline float2 GetShorelineWavesUV(float3 worldPos)
{
	return (worldPos.xz - KWS_ShorelineAreaPosSize.xz) / KWS_ShorelineAreaPosSize.w + 0.5;
}

inline half GetShorelineWavesMask(float2 uv, float distanceToCamera)
{
	uv = 1 - saturate(abs(uv * 2 - 1));
	return saturate((uv.x * uv.y - 0.01) * 5);
}

inline float3 ComputeShorelineOffset(float3 worldPos, float3 waterOffset, float multiplier = 1.0)
{
	if(KWS_ShorelineAreaWavesCount == 0) return waterOffset;

	float2 uv = GetShorelineWavesUV(worldPos);
	if(IsOutsideUV(uv)) return waterOffset;
	float3 beachOffset = KWS_ShorelineWavesDisplacement.SampleLevel(sampler_linear_clamp, uv, 0).xyz;

	float2 orthoDepthUV = GetWaterOrthoDepthUV(worldPos);
	float terrainDepth = GetWaterOrthoDepth(orthoDepthUV);
	if(!IsOutsideUV(orthoDepthUV)) waterOffset = lerp(waterOffset, 0, saturate(terrainDepth + 0.85));

	return waterOffset + beachOffset * multiplier;
}

inline float3 ComputeShorelineNormal(half3 normal, float3 worldPos)
{
	if(KWS_ShorelineAreaWavesCount == 0) return normal;

	float2 uv = GetShorelineWavesUV(worldPos);
	if(IsOutsideUV(uv)) return normal;
	
	float2 waveNormalsRaw = KWS_ShorelineWavesNormal.SampleLevel(sampler_linear_clamp, uv, 0).xy;
	float3 waveNormals = normalize(float3(waveNormalsRaw.x, 1, waveNormalsRaw.y));

	normal = KWS_BlendNormals(normal, waveNormals);
	return normal;
}

inline float3 ComputeWireframeInterpolators(float decodedColor)
{
	if(decodedColor < -0.1) return float3(-1, -1, -1);
	else if(decodedColor < 0.01) return float3(1, 0, 0);
	else if(decodedColor < 0.51) return float3(0, 1, 0);
	return float3(0, 0, 1);
}

inline float3 ComputeWireframe(float3 baryColor, float3 sourceColor)
{
	float minBary = min(baryColor.x, min(baryColor.y, baryColor.z));
	float delta = fwidth(minBary) * 0.5;
	return minBary > delta ? sourceColor : float3(0.95, 0.95, 0.95);
}

inline float3 ComputeDepthFoam(float3 underwaterColor, float fade, float3 worldPos, float shadowMask, float3 waterColor, float exposure)
{
	float2 foamMask = KW_FoamTex.SampleLevel(sampler_linear_repeat, worldPos.xz * KWS_FoamFadeSize.y, 0).xy;
	float foamDepth1 = 1 - saturate(fade * KWS_FoamFadeSize.x);
	float foamDepth2 = 1 - saturate(fade * KWS_FoamFadeSize.x * 3);


	float3 foam = max(foamDepth1 * foamMask.x, foamDepth2 * foamMask.y);
	float3 finalColor = foam * KWS_FoamColor.xyz * lerp(GetAmbientColor(), GetAmbientColor() + GetMainLightColor() , shadowMask) * exposure;
	underwaterColor = lerp(underwaterColor, finalColor, saturate(foam * 10) * KWS_FoamColor.a);

	return underwaterColor;
}