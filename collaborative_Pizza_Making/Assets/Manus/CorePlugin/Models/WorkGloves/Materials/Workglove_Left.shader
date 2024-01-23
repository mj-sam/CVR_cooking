// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:1,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:34075,y:32010,varname:node_2865,prsc:2|diff-9542-OUT,spec-8982-R,gloss-8982-G,normal-5964-RGB,difocc-8982-B;n:type:ShaderForge.SFN_Tex2d,id:7736,x:33242,y:31982,ptovrint:True,ptlb:Albedo,ptin:_Albedo,varname:_Albedo,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a403b3124871d1748878dc6aae6d5b56,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5964,x:33478,y:32590,ptovrint:True,ptlb:Normal,ptin:_Normal,varname:_Normal,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:cb5098e8702aa194b80624ab0b3e7c50,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:8982,x:33478,y:32363,ptovrint:True,ptlb:RMAO,ptin:_RMAO,varname:_RMAO,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3169b8ec55b6da44287fd7d2efc0ec61,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:7445,x:31482,y:31664,ptovrint:False,ptlb:Fabric Hand Color,ptin:_FabricHandColor,varname:_FabricHandColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:9082,x:32012,y:32235,ptovrint:False,ptlb:Fabric Wrist Color,ptin:_FabricWristColor,varname:_FabricWristColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8666667,c2:0.8666667,c3:0.8666667,c4:1;n:type:ShaderForge.SFN_Color,id:6821,x:32012,y:32457,ptovrint:False,ptlb:Fabric Wrist Accent Color,ptin:_FabricWristAccentColor,varname:_FabricWristAccentColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:4363,x:32012,y:32013,ptovrint:False,ptlb:Ruberized Grip Color,ptin:_RuberizedGripColor,varname:_RuberizedGripColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5411765,c2:0.5411765,c3:0.5411765,c4:1;n:type:ShaderForge.SFN_Multiply,id:9687,x:32219,y:32457,varname:node_9687,prsc:2|A-6821-RGB,B-214-B;n:type:ShaderForge.SFN_Multiply,id:1284,x:32219,y:32235,varname:node_1284,prsc:2|A-9082-RGB,B-214-G;n:type:ShaderForge.SFN_Multiply,id:5459,x:32219,y:32013,varname:node_5459,prsc:2|A-4363-RGB,B-214-R;n:type:ShaderForge.SFN_Multiply,id:7193,x:32219,y:31834,varname:node_7193,prsc:2|A-7445-RGB,B-3601-RGB;n:type:ShaderForge.SFN_Multiply,id:5967,x:33480,y:32106,varname:node_5967,prsc:2|A-7736-RGB,B-1962-OUT;n:type:ShaderForge.SFN_Multiply,id:2786,x:32485,y:31812,varname:node_2786,prsc:2|A-2914-OUT,B-7193-OUT;n:type:ShaderForge.SFN_Tex2d,id:214,x:31472,y:32099,ptovrint:True,ptlb:Color Mask 1-3,ptin:_Masks,varname:_Masks,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2277d663fc66b6349b8d60c930d381b9,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3601,x:31472,y:31871,ptovrint:False,ptlb:Color Mask 4,ptin:_ColorMask4,varname:_ColorMask4,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:50bd95ad4a8746141b7797d616eaf0a4,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:9204,x:32738,y:31991,varname:node_9204,prsc:2|A-2786-OUT,B-5459-OUT;n:type:ShaderForge.SFN_Add,id:7576,x:32974,y:32089,varname:node_7576,prsc:2|A-9204-OUT,B-1284-OUT;n:type:ShaderForge.SFN_Add,id:1962,x:33242,y:32208,varname:node_1962,prsc:2|A-7576-OUT,B-9687-OUT;n:type:ShaderForge.SFN_Vector3,id:2914,x:32219,y:31686,varname:node_2914,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Tex2d,id:5430,x:33535,y:31728,ptovrint:False,ptlb:Print,ptin:_Print,varname:_Print,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3af594e1ad43aaf4d8291b4f27570111,ntxv:0,isnm:False|UVIN-4791-OUT;n:type:ShaderForge.SFN_Multiply,id:9542,x:33805,y:31966,varname:node_9542,prsc:2|A-5967-OUT,B-5430-RGB;n:type:ShaderForge.SFN_OneMinus,id:8065,x:32774,y:31586,varname:node_8065,prsc:2|IN-472-U;n:type:ShaderForge.SFN_Lerp,id:7285,x:33012,y:31586,varname:node_7285,prsc:2|A-472-U,B-8065-OUT,T-3209-OUT;n:type:ShaderForge.SFN_Append,id:4791,x:33243,y:31604,varname:BlendLeft,prsc:2|A-7285-OUT,B-472-V;n:type:ShaderForge.SFN_ToggleProperty,id:3209,x:32774,y:31745,ptovrint:False,ptlb:Flip Print,ptin:_FlipPrint,varname:_FlipPrint,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True;n:type:ShaderForge.SFN_TexCoord,id:472,x:32510,y:31582,varname:node_472,prsc:2,uv:1,uaff:False;proporder:7736-5964-8982-214-3601-5430-3209-7445-9082-4363-6821;pass:END;sub:END;*/

Shader "Shader Forge/ManusVR_Workglove_Left" {
    Properties {
        _Albedo ("Albedo", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _RMAO ("RMAO", 2D) = "white" {}
        _Masks ("Color Mask 1-3", 2D) = "white" {}
        _ColorMask4 ("Color Mask 4", 2D) = "white" {}
        _Print ("Print", 2D) = "white" {}
        [MaterialToggle] _FlipPrint ("Flip Print", Float ) = 1
        _FabricHandColor ("Fabric Hand Color", Color) = (1,1,1,1)
        _FabricWristColor ("Fabric Wrist Color", Color) = (0.8666667,0.8666667,0.8666667,1)
        _RuberizedGripColor ("Ruberized Grip Color", Color) = (0.5411765,0.5411765,0.5411765,1)
        _FabricWristAccentColor ("Fabric Wrist Accent Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _RMAO; uniform float4 _RMAO_ST;
            uniform float4 _FabricHandColor;
            uniform float4 _FabricWristColor;
            uniform float4 _FabricWristAccentColor;
            uniform float4 _RuberizedGripColor;
            uniform sampler2D _Masks; uniform float4 _Masks_ST;
            uniform sampler2D _ColorMask4; uniform float4 _ColorMask4_ST;
            uniform sampler2D _Print; uniform float4 _Print_ST;
            uniform fixed _FlipPrint;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _RMAO_var = tex2D(_RMAO,TRANSFORM_TEX(i.uv0, _RMAO));
                float gloss = 1.0 - _RMAO_var.g; // Convert roughness to gloss
                float perceptualRoughness = _RMAO_var.g;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _RMAO_var.r;
                float specularMonochrome;
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float4 _ColorMask4_var = tex2D(_ColorMask4,TRANSFORM_TEX(i.uv0, _ColorMask4));
                float4 _Masks_var = tex2D(_Masks,TRANSFORM_TEX(i.uv0, _Masks));
                float2 BlendLeft = float2(lerp(i.uv1.r,(1.0 - i.uv1.r),_FlipPrint),i.uv1.g);
                float4 _Print_var = tex2D(_Print,TRANSFORM_TEX(BlendLeft, _Print));
                float3 diffuseColor = ((_Albedo_var.rgb*((((float3(1,1,1)*(_FabricHandColor.rgb*_ColorMask4_var.rgb))+(_RuberizedGripColor.rgb*_Masks_var.r))+(_FabricWristColor.rgb*_Masks_var.g))+(_FabricWristAccentColor.rgb*_Masks_var.b)))*_Print_var.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                indirectDiffuse *= _RMAO_var.b; // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _RMAO; uniform float4 _RMAO_ST;
            uniform float4 _FabricHandColor;
            uniform float4 _FabricWristColor;
            uniform float4 _FabricWristAccentColor;
            uniform float4 _RuberizedGripColor;
            uniform sampler2D _Masks; uniform float4 _Masks_ST;
            uniform sampler2D _ColorMask4; uniform float4 _ColorMask4_ST;
            uniform sampler2D _Print; uniform float4 _Print_ST;
            uniform fixed _FlipPrint;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _RMAO_var = tex2D(_RMAO,TRANSFORM_TEX(i.uv0, _RMAO));
                float gloss = 1.0 - _RMAO_var.g; // Convert roughness to gloss
                float perceptualRoughness = _RMAO_var.g;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _RMAO_var.r;
                float specularMonochrome;
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float4 _ColorMask4_var = tex2D(_ColorMask4,TRANSFORM_TEX(i.uv0, _ColorMask4));
                float4 _Masks_var = tex2D(_Masks,TRANSFORM_TEX(i.uv0, _Masks));
                float2 BlendLeft = float2(lerp(i.uv1.r,(1.0 - i.uv1.r),_FlipPrint),i.uv1.g);
                float4 _Print_var = tex2D(_Print,TRANSFORM_TEX(BlendLeft, _Print));
                float3 diffuseColor = ((_Albedo_var.rgb*((((float3(1,1,1)*(_FabricHandColor.rgb*_ColorMask4_var.rgb))+(_RuberizedGripColor.rgb*_Masks_var.r))+(_FabricWristColor.rgb*_Masks_var.g))+(_FabricWristAccentColor.rgb*_Masks_var.b)))*_Print_var.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform sampler2D _RMAO; uniform float4 _RMAO_ST;
            uniform float4 _FabricHandColor;
            uniform float4 _FabricWristColor;
            uniform float4 _FabricWristAccentColor;
            uniform float4 _RuberizedGripColor;
            uniform sampler2D _Masks; uniform float4 _Masks_ST;
            uniform sampler2D _ColorMask4; uniform float4 _ColorMask4_ST;
            uniform sampler2D _Print; uniform float4 _Print_ST;
            uniform fixed _FlipPrint;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float4 _ColorMask4_var = tex2D(_ColorMask4,TRANSFORM_TEX(i.uv0, _ColorMask4));
                float4 _Masks_var = tex2D(_Masks,TRANSFORM_TEX(i.uv0, _Masks));
                float2 BlendLeft = float2(lerp(i.uv1.r,(1.0 - i.uv1.r),_FlipPrint),i.uv1.g);
                float4 _Print_var = tex2D(_Print,TRANSFORM_TEX(BlendLeft, _Print));
                float3 diffColor = ((_Albedo_var.rgb*((((float3(1,1,1)*(_FabricHandColor.rgb*_ColorMask4_var.rgb))+(_RuberizedGripColor.rgb*_Masks_var.r))+(_FabricWristColor.rgb*_Masks_var.g))+(_FabricWristAccentColor.rgb*_Masks_var.b)))*_Print_var.rgb);
                float specularMonochrome;
                float3 specColor;
                float4 _RMAO_var = tex2D(_RMAO,TRANSFORM_TEX(i.uv0, _RMAO));
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _RMAO_var.r, specColor, specularMonochrome );
                float roughness = _RMAO_var.g;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
