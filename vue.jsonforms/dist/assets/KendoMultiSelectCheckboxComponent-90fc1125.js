import{j as oe,D as ie,d4 as re,u as ce,l as H,G as C,Z as q,aX as ue,A as v,bS as me,d8 as J,d9 as S,_ as pe,o as c,q as G,w as o,n as M,a0 as Q,r as U,x as f,m as N,y as K,P as Y,L as he,bp as ve,cK as fe,da as Fe,E as ge,bW as ke,B as ye}from"./index-9d6d511f.js";const Ie=oe({name:"KendoMultiSelectCheckboxComponent",props:{items:{type:Array,default:()=>[]},value:{type:Array,default:()=>[]},name:{type:String,default:""},id:String,label:{type:String,default:""},placeholder:{type:String,default:"Please Type or Select"},hint:{type:String,default:""},disabled:{type:Boolean,default:!1},isRequired:{type:Boolean,default:!0},valuePrimitive:{type:Boolean,default:!1},loading:{type:Boolean,default:!1},isValueReactive:{type:Boolean,default:!1},isDataLoadedCompletely:{type:Boolean,default:!1}},setup(e,{emit:u}){const O="label",V="value",_="checkField",x="checkIndeterminateField",F="items",g="expanded",k=ie.resolve(re.name),B=ce(),{addComponentValidationValue:E}=B,y=H("kendoForm",{default:{}}),i=H("$eventBusService"),d=C(""),n=C(!1),I=C(e.items),s=C(e.value),P=q(()=>I.value.map(t=>t.value).filter(t=>t!==void 0)),ee=q(()=>j(I.value,{dataItemKey:V,subItemsField:F,checkField:_,checkIndeterminateField:x,expandField:g,value:s.value,expanded:P.value})),te=(t,a)=>{const l={};return t.forEach(m=>{l[a(m)]=!0}),l},W=(t,a)=>{const{keyGetter:l,subItemGetter:m,subItemSetter:L,checkSetter:T,expandedSetter:A,checkIndeterminateSetter:$,valueMap:w,expandedMap:b}=a;if(!t||!t.length)return[t,!1];let p=!1;return[[...t].map(h=>{const[se,Z]=W(m(h),a),R=w[l(h)];(R||Z)&&(p=!0);const r={...h};return A(r,b[l(r)]),L(r,se),T(r,R),$(r,!R&&Z),r}),p]},j=(t,a)=>{const{subItemsField:l="items",checkField:m="checkField",checkIndeterminateField:L="checkIndeterminateField",expandField:T="expanded",dataItemKey:A,value:$,expanded:w}=a,b=J(A),p={};w.forEach(h=>p[h.toString()]=!0);const[X]=W(t,{valueMap:te($,b),expandedMap:p,keyGetter:b,expandedSetter:S(T),subItemGetter:J(l),subItemSetter:S(l),checkSetter:S(m),checkIndeterminateSetter:S(L),dataItemKey:"",value:[],expanded:[]});return X},ae=t=>{s.value=t.value,u("onValueChange",t.value),i.emit(v.formFieldChangedEvent),i.emit(v.formFieldPageLevelChangedEvent)},ne=t=>{s.value.splice(t,1),u("onValueChange",s.value),i.emit(v.formFieldChangedEvent),i.emit(v.formFieldPageLevelChangedEvent)};ue(()=>{n.value=D(s.value)==""});const le=(t,a)=>t.length>a?t.substring(0,a-3)+"...":t,de=q(()=>s.value.length===1),D=t=>{d.value="",e.isValueReactive&&!t&&e.value&&(t=e.value),k.setFieldName(e.label||e.name);const a=k.validate(t,{[v.RequiredKey]:e.isRequired});return z(a),d.value=a,a},z=t=>{if(!e.id)return;const a=e.id;let l={};l[a]=t,E(a,l)};return me(s,t=>{Array.isArray(t)&&(s.value=t),y.errors[e.name]=D(t)},{immediate:!0,deep:!0}),{textField:O,dataItemKey:V,checkField:_,checkIndeterminateField:x,subItemsField:F,expandField:g,dataItems:I,expanded:P,processMultiSelectTreeData:j,treeData:ee,onChange:ae,addEllipsis:le,removeItem:ne,watchValueLength:de,errorMessage:d,validate:D,addIdKeyAndErrorValue:z,kendoForm:y}}});const be={key:0,class:"fineprint ms-1"},Ce={key:0,class:"k-chip"},Se=["title"],Me={key:1};function Ke(e,u,O,V,_,x){const F=he,g=ve,k=fe,B=Fe,E=ge,y=ke,i=ye;return c(),G(i,{id:e.id,name:e.name,component:"template",validator:e.validate},{template:o(({props:d})=>[M(y,{class:Q(d.class)},{default:o(()=>[e.label?(c(),G(F,{key:0,"editor-id":e.id,disabled:e.disabled,valid:!(d.touched&&e.errorMessage),class:"control-label mb-2"},{default:o(()=>[U(f(e.label)+" ",1),e.isRequired?K("",!0):(c(),N("span",be," (Optional) "))]),_:2},1032,["editor-id","disabled","valid"])):K("",!0),M(B,{class:Q(["multi-select-checkbox",{"has-selected":e.value.length>0}]),dataItems:e.treeData,value:e.value,placeholder:"Please Select",textField:e.textField,dataItemKey:e.dataItemKey,checkField:e.checkField,checkIndeterminateField:e.checkIndeterminateField,subItemsField:e.subItemsField,expandField:e.expandField,valid:!(d.touched&&e.errorMessage),tag:"tag",popupOpen:e.watchValueLength,keepPopupOpen:!0,onChange:u[0]||(u[0]=n=>e.onChange(n))},{item:o(({props:n})=>[Y("span",null,f(n.item.text),1)]),tag:o(({props:n})=>[n.index===0||n.index===1&&e.value.length>1?(c(),N("li",Ce,[M(k,{"anchor-element":"target",position:"top"},{default:o(()=>[M(g,{"theme-color":"white",size:"lg",class:"font-weight-semi-bold","text-class":"is-truncated",closeable:"",onClose:I=>e.removeItem(n.index)},{default:o(()=>[Y("span",{class:"is-truncated",title:n.tagData.text},f(e.addEllipsis(n.tagData.text,14)),9,Se)]),_:2},1032,["onClose"])]),_:2},1024)])):n.index>1?(c(),N("li",Me,"+"+f(e.value.length-2),1)):K("",!0)]),_:2},1032,["class","dataItems","value","textField","dataItemKey","checkField","checkIndeterminateField","subItemsField","expandField","valid","popupOpen"]),d.touched&&e.errorMessage?(c(),G(E,{key:1,style:{"margin-top":"8px"}},{default:o(()=>[U(f(e.errorMessage),1)]),_:1})):K("",!0)]),_:2},1032,["class"])]),_:1},8,["id","name","validator"])}const _e=pe(Ie,[["render",Ke]]);export{_e as default};
