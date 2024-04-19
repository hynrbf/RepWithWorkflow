import{j as pe,C as ve,Z as N,G as P,aY as _e,a3 as J,H as he,o as d,m as g,J as Y,K as Z,P as o,x as y,n as w,w as _,$ as z,q as f,y as H,A as Q,aZ as fe,a$ as Ce,Y as ye,a4 as ge,Q as Ie,N as Ve,b0 as Pe,a5 as Ae,a6 as $e,dn as be,_ as ke}from"./index-9d6d511f.js";import{_ as Le}from"./InfoTableComponent.vue_vue_type_script_setup_true_lang-a9fe18e7.js";const B=k=>(Ae("data-v-74627848"),k=k(),$e(),k),xe={key:0},Te={class:"DescTable"},Fe=B(()=>o("tr",null,[o("th",{width:"30%"},"Product Name"),o("th",{class:"text-center"},"Projected Annual Fee Income"),o("th",{class:"text-center"},"Projected Annual Commission Income"),o("th",null,"Limitations")],-1)),Oe={class:"text-center"},we={class:"text-center"},Re={key:1},Ne=B(()=>o("div",{class:"mb-3"},null,-1)),Be={class:"ActivityCollapsiblePanel-title"},qe={key:1},Ee={class:"mt-0"},Ue={key:0},je=["onClick"],Se={key:1},Ge=B(()=>o("div",{style:{width:"20px"}},null,-1)),Ke=[Ge],Me=B(()=>o("div",{class:"py-1"},[o("strong",null,"Total")],-1)),De={class:"py-1"},Je={class:"TotalText"},Ye={class:"py-1"},Ze={class:"TotalText"},ze=pe({__name:"Activities",props:{modelValue:{default:()=>[]},viewMode:{type:Boolean}},emits:["update:modelValue"],setup(k,{emit:W}){const X=k,{modelValue:ee,viewMode:ne}=ve(X),u=N({get(){return ee.value.reduce((t,n)=>({...t,[n.productId]:n}),{})},set(t){W("update:modelValue",Object.values(t))}}),I=P(new _e(0,"EUR")),U=()=>{let t=[];const n=localStorage.getItem(Q.customerProductsKey);return n&&(t=JSON.parse(n)),t.find(r=>r.pageName=="Appointed Representatives")??new be},te=()=>U().categories.map(n=>{var r;return{id:(r=n.name)==null?void 0:r.toLowerCase().replace(/ /g,"-"),title:n.displayText}}),ae=()=>{let t=[];return U().categories.forEach(n=>{var r;let a=(r=n.name)==null?void 0:r.toLowerCase().replace(/ /g,"-");n.products.forEach(V=>{var C;t.push({id:((C=V.name)==null?void 0:C.toLowerCase().replace(/ /g,"-"))??"",typeId:a??"",title:V.displayText??""})})}),t.length==0&&(t=J.getARProducts()),t},L=P(te()),j=P(J.getARActivityProducts()),x=P(ae()),oe=N(()=>[{field:"product",title:"Product"},{field:"appointment",title:"Appointed"},{field:"annualFeeIncome",title:"Projected Annual Fee Income"},{field:"annualCommissionIncome",title:"Projected Annual Commission Income"},{field:"limitations",title:"Limitations"},{field:"action",title:" "}]),le=N(()=>(L.value.length>0?L.value:j.value).map(({id:n,title:a,icon:r})=>({id:`activity-nav-${n}`,anchorTo:`panel-${n}`,label:a,icon:r,active:n==="mortgage-broking"}))),se=N(()=>(L.value.length>0?L.value:j.value).map(({id:n,title:a})=>({id:n,title:a,content:n,contentClass:"p-0"}))),A=t=>{const n=x.value.filter(a=>a.typeId===t).map(a=>({product:a.id,annualFeeIncome:{amount:0,currency:"GBP",symbol:"£"},annualCommissionIncome:{amount:0,currency:"GBP",symbol:"£"},limitations:"",hasLimitation:!1,appointment:!1}));return n.push({product:"add-more",annualFeeIncome:{amount:0,currency:"GBP",symbol:"£"},annualCommissionIncome:{amount:0,currency:"GBP",symbol:"£"},limitations:"",hasLimitation:!1,appointment:!1}),n},$=(t,n)=>{const a=u.value[t];a?u.value={...u.value,[t]:{...a,...n}}:u.value={...u.value,[t]:{productId:t,...n}}},S=t=>{const n=Object.assign({},u.value);delete n[t],u.value=n},q=t=>x.value.find(n=>n.id===t),ie=t=>{x.value=[...x.value,{...t}]},T=t=>"£ "+t.toFixed(2).replace(/\d(?=(\d{3})+\.)/g,"$&,"),R=(t,n)=>Object.values(u.value).reduce((a,r)=>{var C;const V=q(r.productId);return!V||t!==V.typeId?a:a+(((C=r[n])==null?void 0:C.amount)||0)},0),G=P(),F=P([]),ue=(t,n)=>{n?F.value=[...F.value,t]:F.value=F.value.filter(a=>a!==t),x.value.forEach(a=>{a.typeId===t&&setTimeout(()=>{n?$(a.id,{isAppointed:n}):S(a.id)},200)})},K=t=>{const n=t.filter(a=>a.active);if(n.length>0){const a=n[0].anchorTo;a!==void 0&&a!==""&&G.value.expand(a.replace(/^panel-/,"").trim())}},p=P({}),b=(t,n)=>{p.value={...p.value,[t]:{...p.value[t]??{},...n}}},ce=t=>{const n=p.value[t];if(!n)return;const a=n.title.replace(/ +/g,"-").toLowerCase();u.value={...u.value,[a]:{productId:a,annualFeeIncome:n.annualFeeIncome,annualCommissionIncome:n.annualCommissionIncome,isAppointed:!0,hasLimitation:n.hasLimitation,isNewProduct:!0}},ie({id:a,typeId:t,title:n.title}),delete p.value[t]},h=t=>`${Q.appointedRepresentativesRoute}${t}`.replace(/\s+/g,"").replace("/","");return(t,n)=>{const a=fe,r=Ce,V=ye,C=ge,M=Ie,de=Ve,me=Le,re=Pe;return he(ne)?(d(),g("div",xe,[(d(!0),g(Y,null,Z(L.value,m=>(d(),g("dl",{key:`dl-${m.id}`,class:"DescList"},[o("dt",null,y(m.title)+" | "+y(T(R(m.id,"annualFeeIncome")+R(m.id,"annualCommissionIncome"))),1),o("dd",null,[o("table",Te,[Fe,(d(!0),g(Y,null,Z(A(m.id),e=>{var l,s,c,i,v,O;return d(),g("tr",{key:`dt-${e.product}`},[o("td",null,y(A(m.id).length===1?"-":e.product==="add-more"?"":(l=q(e.product))==null?void 0:l.title),1),o("td",Oe,y(A(m.id).length===1?"-":e.product==="add-more"?"":T(((c=(s=u.value[e.product])==null?void 0:s.annualFeeIncome)==null?void 0:c.amount)??0)),1),o("td",we,y(A(m.id).length===1?"-":e.product==="add-more"?"":T(((v=(i=u.value[e.product])==null?void 0:i.annualCommissionIncome)==null?void 0:v.amount)??0)),1),o("td",null,y(A(m.id).length===1?"-":e.product==="add-more"?"":(O=u.value[e.product])==null?void 0:O.limitations),1)])}),128))])])]))),128))])):(d(),g("div",Re,[w(a,{items:le.value,class:"ActivitiesNavPill is-sticky-top is-bg-white",anchorable:"","onUpdate:items":K},null,8,["items"]),Ne,w(re,{ref_key:"ActivityCollapsiblePanel",ref:G,class:"ActivityCollapsiblePanel activity-modal",items:se.value,accordion:!1},{title:_(({item:m})=>[o("div",Be,[o("span",null,y(m.title),1),w(r,{name:`toggle-${m.id}`,id:h(`-toggle-${m.id}`),"is-required":!1,"no-text":"",value:F.value.includes(m.content),onClick:n[0]||(n[0]=z(()=>{},["stop"])),onOnValueChange:e=>{ue(m.content,e),K(m.id)}},null,8,["name","id","value","onOnValueChange"])])]),content:_(({item:m})=>[w(me,{class:"ActivityInfoTable",id:m.content,columns:oe.value,data:[{items:A(m.content),footer:!0}]},{"cell-product":_(({item:e,id:l})=>{var s,c;return[e.product==="add-more"?(d(),f(V,{key:0,name:"add-more",placeholder:"Add Product","model-value":((s=p.value[l])==null?void 0:s.title)??"","onUpdate:modelValue":i=>b(l??"",{title:i})},null,8,["model-value","onUpdate:modelValue"])):(d(),g("span",qe,y((c=q(e.product))==null?void 0:c.title),1))]}),"cell-appointment":_(({item:e,id:l})=>{var s,c;return[e.product==="add-more"?(d(),f(r,{key:0,name:`appointed-${e.product}`,id:h(`-appointed-${e.product}`),"is-required":!1,"no-text":"",value:((s=p.value[l])==null?void 0:s.isAppointed)??!1,onOnValueChange:i=>b(l??"",{isAppointed:i})},null,8,["name","id","value","onOnValueChange"])):(d(),f(r,{key:1,name:`appointed-${e.product}`,id:h(`-appointed-${e.product}`),"is-required":!1,"no-text":"",value:((c=u.value[e.product])==null?void 0:c.isAppointed)??!1,onOnValueChange:i=>{i?$(e.product,{isAppointed:i}):S(e.product)}},null,8,["name","id","value","onOnValueChange"]))]}),"cell-annualFeeIncome":_(({item:e,id:l})=>{var s,c,i;return[e.product==="add-more"?(d(),f(C,{key:0,name:`annualFeeIncome-${e.product}`,id:h(`-annualFeeIncome-${e.product}`),placeholder:"Enter Amount",hasLabel:!1,value:(s=p.value[l])==null?void 0:s.annualFeeIncome,minimumValue:I.value,isValueReactive:!0,onOnValueChange:v=>b(l,{annualFeeIncome:v??I.value})},null,8,["name","id","value","minimumValue","onOnValueChange"])):(d(),f(C,{key:1,name:`annualFeeIncome-${e.product}`,id:h(`-annualFeeIncome-${e.product}`),placeholder:"Enter Amount",hasLabel:!1,disabled:!((c=u.value[e.product])!=null&&c.isAppointed),value:(i=u.value[e.product])==null?void 0:i.annualFeeIncome,minimumValue:I.value,isValueReactive:!0,onOnValueChange:v=>$(e.product,{annualFeeIncome:v??I.value})},null,8,["name","id","disabled","value","minimumValue","onOnValueChange"]))]}),"cell-annualCommissionIncome":_(({item:e,id:l})=>{var s,c,i;return[e.product==="add-more"?(d(),f(C,{key:0,name:`annualCommissionIncome-${e.product}`,id:h(`-annualCommissionIncome-${e.product}`),placeholder:"Enter Amount",hasLabel:!1,value:(s=p.value[l])==null?void 0:s.annualCommissionIncome,minimumValue:I.value,isValueReactive:!0,onOnValueChange:v=>b(l,{annualCommissionIncome:v??I.value})},null,8,["name","id","value","minimumValue","onOnValueChange"])):(d(),f(C,{key:1,name:`annualCommissionIncome-${e.product}`,id:h(`-annualCommissionIncome-${e.product}`),placeholder:"Enter Amount",hasLabel:!1,disabled:!((c=u.value[e.product])!=null&&c.isAppointed),value:(i=u.value[e.product])==null?void 0:i.annualCommissionIncome,minimumValue:I.value,isValueReactive:!0,onOnValueChange:v=>$(e.product,{annualCommissionIncome:v??I.value})},null,8,["name","id","disabled","value","minimumValue","onOnValueChange"]))]}),"cell-limitations":_(({item:e,id:l})=>{var s,c;return[e.product==="add-more"?(d(),f(r,{key:0,name:`limitation-${e.product}`,id:h(`-limitation-${e.product}`),"is-required":!1,"no-text":"",value:((s=p.value[l])==null?void 0:s.hasLimitation)??!1,onOnValueChange:i=>b(l,{hasLimitation:i})},null,8,["name","id","value","onOnValueChange"])):(d(),f(r,{key:1,name:`limitation-${e.product}`,id:h(`-limitation-${e.product}`),"is-required":!1,"no-text":"",value:((c=u.value[e.product])==null?void 0:c.hasLimitation)??!1,onOnValueChange:i=>$(e.product,{hasLimitation:i})},null,8,["name","id","value","onOnValueChange"]))]}),"expanded-content":_(({item:e,item:{product:l},id:s})=>{var c,i,v,O,D;return[o("div",Ee,[l==="add-more"&&((c=p.value[s])!=null&&c.hasLimitation)?(d(),f(M,{key:0,id:h(`-limitations-${l}`),name:`limitations-${l}`,class:"col-custom-activities",placeholder:"Please Type Limitations",value:(i=p.value[s])==null?void 0:i.limitations,isRequired:!1,"model-value":((v=p.value[s])==null?void 0:v.limitations)??"","onUpdate:modelValue":E=>b(s,{limitations:E})},null,8,["id","name","value","model-value","onUpdate:modelValue"])):H("",!0),(O=u.value[e.product])!=null&&O.hasLimitation?(d(),f(M,{key:1,id:h(`-limitations-${e.product}`),name:`limitations-${e.product}`,class:"col-custom-activities",placeholder:"Please Type Limitations",value:(D=u.value[e.product])==null?void 0:D.limitations,isRequired:!1,"onUpdate:modelValue":E=>$(e.product,{limitations:E})},null,8,["id","name","value","onUpdate:modelValue"])):H("",!0)])]}),"cell-action":_(({item:e,id:l})=>[e.product==="add-more"?(d(),g("div",Ue,[o("button",{class:"AddButton",onClick:z(s=>ce(l??""),["prevent"])},[w(de,{symbol:"add-circle-27",size:"20"})],8,je)])):(d(),g("div",Se,Ke))]),"footer-cell-product":_(()=>[Me]),"footer-cell-annualFeeIncome":_(({id:e})=>[o("div",De,[o("span",Je,y(T(R(e,"annualFeeIncome"))),1)])]),"footer-cell-annualCommissionIncome":_(({id:e})=>[o("div",Ye,[o("span",Ze,y(T(R(e,"annualCommissionIncome"))),1)])]),_:2},1032,["id","columns","data"])]),_:1},8,["items"])]))}}});const We=ke(ze,[["__scopeId","data-v-74627848"]]);export{We as default};
