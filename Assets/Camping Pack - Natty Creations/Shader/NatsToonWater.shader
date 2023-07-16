Shader "CampingPack/NatsToonWater" {
    Properties{
      _MainTex("Texture", 2D) = "white" {}
     _Tint("Tint",  Color) = (1,1,1,1)
      _DistortMap("Texture Distort", 2D) = "white" {}
     _DistortAmount("Distort Amount",Range(0, 10)) = 0.25
        _Amplitude("Wave Frequency",Range(0, 10)) = 0.25
        _Scale("Wave Height",Range(-1, 1)) = 0.25
        _Speed("Water Speed",Range(0, 1)) = 0.25
        _Alpha("Alpha", Range(0, 1)) = 0.25
        _Smoothness("Glossiness",Range(0, 1)) = 0.25


    } 

        SubShader{
         Tags {"Queue" = "Transparent"  "RenderType" = "Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
          CGPROGRAM
          #pragma surface surf Standard vertex:vert alpha:fade
          #pragma target 3.0 


        float _Scale, _Speed, _Alpha,_Amplitude,_Smoothness,_DistortAmount;
        fixed4 _Tint;
      struct Input {
              float2 uv_MainTex;
              float3 customValue;
              float3 viewDir;
          };

     
      sampler2D _MainTex, _DistortMap;

      // Vertex modifier function

      void vert(inout appdata_full v, out Input o) {
          UNITY_INITIALIZE_OUTPUT(Input, o);
          half offsetvert = (v.vertex.x + (v.vertex.z * v.vertex.z)) * _Amplitude;
          half value0 = (_Scale * 0.5f) * sin(_Time.w * (_Speed * 0.3f) + offsetvert);
          half value2 = (_Scale * 0.5f) * sin(_Time.w * (_Speed * 0.3f) + offsetvert)*0.1f;
          float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
         // o.customColor = abs(v.normal);
          v.vertex.y += value0;
          v.normal.y += value0; 
          o.customValue -= value0  ;
      }

      // Surface shader function
      void surf(Input IN, inout SurfaceOutputStandard o) {

          float2 distortVector = tex2D(_DistortMap, IN.uv_MainTex).rgb;
          float2 uv = IN.uv_MainTex - distortVector * _DistortAmount;
          float2 uv2 = IN.uv_MainTex;
          uv += _Time.y * _Speed * 0.05f;
          uv2 += _Time.y * _Speed * 0.02f;
        //  float2 uv = FlowUV(IN.uv_MainTex, distortVector, _Time.y);
          fixed4 c = tex2D(_MainTex, uv);
  
          o.Albedo = c.rgb * _Tint;
          o.Normal = UnpackNormal(tex2D(_DistortMap, uv2));
          //o.Smoothness = _Smoothness * IN.viewDir;
          float4 _FoamColour = (1, 1, 1, 1);
          o.Smoothness = -_Smoothness * 10 * pow(0.2f + .5f, 2) + (_FoamColour * IN.customValue );
          o.Alpha = _Alpha;
      }
      ENDCG
      }
          
          

}
