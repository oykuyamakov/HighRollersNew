// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "OutlineShader" 
{
	Properties
    { 
    	_Width ("Width", Float ) = 1
	    _Color ("Color", Color) = (1,1,1,1)
        _BaseMap("Base Map", 2D) = "white"
    }
}