Shader "Bolt/Area Of Interest Spheres" {
	Properties {
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	}
	SubShader {
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
	}

		Cull Off
		
		CGPROGRAM
			#pragma surface surf BlinnPhong decal:add vertex:vert nolightmap

			struct Input {
				float dummy;
			};

			void vert (inout appdata_full v) {
				v.vertex.xyz += (v.normal * 0.15f);
			}

			void surf (Input IN, inout SurfaceOutput o) {
				o.Albedo = _SpecColor * 0.125;
				o.Gloss = 0.75;
				o.Specular = 0.0125;
				o.Alpha = 0;
			}
		ENDCG
	}
}