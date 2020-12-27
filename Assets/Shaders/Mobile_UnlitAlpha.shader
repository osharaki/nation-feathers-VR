///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//   ____ ___ _     ___ ____ ___  _   _   ____  ____   ___ ___ ____
//  / ___|_ _| |   |_ _/ ___/ _ \| \ | | |  _ \|  _ \ / _ \_ _|  _ \
//  \___ \| || |    | | |  | | | |  \| | | | | | |_) | | | | || | | |
//   ___) | || |___ | | |__| |_| | |\  | | |_| |  _ <| |_| | || |_| |
//  |____/___|_____|___\____\___/|_| \_| |____/|_| \_\\___/___|____/
//
//    MOBILE: UNLIT WITH ALPHA
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Shader "SiliconDroid/Mobile_UnlitAlpha"
{
	Properties
	{
		_MainTex("Main Texture (RGBA)", 2D) = "white" {}
	}

		Category
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off
		Cull Back
		ZWrite Off
		ZTest LEqual
		Fog{ Color(0,0,0,0) }

		BindChannels
	{
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}

		SubShader
	{
		Pass
	{
		SetTexture[_MainTex]
	{
		combine texture * primary
	}
	}
	}
	}
		Fallback "Transparent/Cutout/VertexLit"
}