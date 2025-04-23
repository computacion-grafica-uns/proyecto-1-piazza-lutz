Shader "ShaderBasico1"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata{
                float4 vertex : POSITION;
                fixed4 color : COLOR;
            };

            struct v2f{
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.color = v.color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i): SV_TARGET
            {
                return (i.color);
            }
            ENDCG

        }
    }
}
