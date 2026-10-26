#include "pch-cpp.hpp"






struct Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA;
struct DataContractAttribute_tD065D7D14CC8AA548815166AB8B8210D1B3C699F;
struct DataMemberAttribute_t8AE446BE9032B9BC8E7B2EDC785F5C6FA0E5BB73;
struct String_t;



IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
struct U3CModuleU3E_t5CFA55679A8E9D2525AFBC9C50BEC051BEA21310 
{
};
struct Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA  : public RuntimeObject
{
};
struct String_t  : public RuntimeObject
{
	int32_t ____stringLength;
	Il2CppChar ____firstChar;
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F  : public RuntimeObject
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_pinvoke
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_com
{
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22 
{
	bool ___m_value;
};
struct DataContractAttribute_tD065D7D14CC8AA548815166AB8B8210D1B3C699F  : public Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA
{
};
struct DataMemberAttribute_t8AE446BE9032B9BC8E7B2EDC785F5C6FA0E5BB73  : public Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA
{
	String_t* ___name;
	bool ___isNameSetExplicitly;
	int32_t ___order;
	bool ___emitDefaultValue;
};
struct Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C 
{
	int32_t ___m_value;
};
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915 
{
	union
	{
		struct
		{
		};
		uint8_t Void_t4861ACF8F4594C3437BB48B6E56783494B843915__padding[1];
	};
};
struct String_t_StaticFields
{
	String_t* ___Empty;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22_StaticFields
{
	String_t* ___TrueString;
	String_t* ___FalseString;
};
#ifdef __clang__
#pragma clang diagnostic pop
#endif



IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2 (Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA* __this, const RuntimeMethod* method) ;
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 75419
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void DataContractAttribute__ctor_mAFB75FDCDDBB5925CCE916496408E843E9DDB6BC (DataContractAttribute_tD065D7D14CC8AA548815166AB8B8210D1B3C699F* __this, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 75420
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void DataMemberAttribute__ctor_m077F867534BAD9865D6DD156A8FBBB4068283A42 (DataMemberAttribute_t8AE446BE9032B9BC8E7B2EDC785F5C6FA0E5BB73* __this, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		__this->___order = (-1);
		__this->___emitDefaultValue = (bool)1;
		Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2(__this, NULL);
		return;
	}
}
// Method Definition Index: 75421
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void DataMemberAttribute_set_Name_mCF9CB9ED10BD17E99D79CC5CA33431F2135971DF (DataMemberAttribute_t8AE446BE9032B9BC8E7B2EDC785F5C6FA0E5BB73* __this, String_t* ___0_value, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		String_t* L_0 = ___0_value;
		__this->___name = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___name), (void*)L_0);
		__this->___isNameSetExplicitly = (bool)1;
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
