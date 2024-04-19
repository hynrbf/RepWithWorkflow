import{_ as ft}from"./ModalComponent.vue_vue_type_script_setup_true_lang-fe9b98ed.js";import{_ as Lt}from"./OverlayLoader-e5a9021a.js";import{j as R,D as ct,I as rt,C as q,dh as Nt,Z as S,o as c,m as p,bg as vt,bm as ht,P as t,n as o,H as s,bO as nt,b5 as at,_ as J,G as N,aX as Rt,J as W,K as Q,$ as x,aB as gt,a0 as dt,N as K,b1 as st,bI as yt,bS as jt,x as I,bc as Z,w as l,r as P,q as O,bo as Bt,W as mt,bd as Ot,A as V,a5 as Ct,a6 as bt,bN as Ft,O as ut,b7 as Ht,bR as Ut,bY as xt,b0 as Vt,M as Wt,y as it,b6 as qt,as as lt,au as Jt,av as Kt,c8 as Yt}from"./index-9d6d511f.js";import{u as Gt,F as ot,_ as Xt,c as Zt,b as Qt}from"./MediaLabelComponent-feae136c.js";import{u as te}from"./MarketingAndFinancialPromotions-3f8cb2b4.js";import{u as ee}from"./useFinancialPromotionStore-bdfe7e68.js";import{u as oe,l as ne,a as se,b as kt,C as D,_ as ae,c as ie}from"./file-xls-fcdae677.js";import"./StatusLabelComponent.vue_vue_type_script_setup_true_lang-90f1f182.js";import"./GridCardTable-248623f7.js";import"./DateTimeComponent-2c5db54c.js";const le={class:"PromotionContent"},ce={class:"PromotionContent-editor"},re={class:"PromotionContent-content is-scrollable-y"},de=["innerHTML"],me=R({__name:"PromotionContent",props:{userId:{default:""},users:{default:()=>[]},content:{default:""},rawContent:{default:""},suggestions:{default:()=>[]},commentThreads:{default:()=>[]},editting:{type:Boolean,default:!1}},emits:["update"],setup(v,{expose:k,emit:g}){const e=v,m=ct.resolve(rt.name),{content:y,rawContent:C,suggestions:_,commentThreads:w,userId:$,users:i}=q(e),a=Nt(),b=S(()=>({classicWithTrackChanges:{currentUser:$.value,users:at(i.value),suggestions:B(at(_.value),!0),commentThreads:at(w.value)},toolbar:["heading","|","fontsize","fontfamily","|","bold","italic","underline","strikethrough","removeFormat","|","numberedList","bulletedList","|","undo","redo","|","link"]})),{Editor:E,onInit:j}=oe("ClassicWithTrackChangesEditor");j(d=>{var h,M;d.execute("trackChanges"),(M=(h=d.commands.get("trackChanges"))==null?void 0:h.forceDisabled)==null||M.call(h,"suggestionsMode"),a.value=d});const T=()=>{if(!a.value)return;const d=a.value,h=d.plugins.get("TrackChanges"),M=d.plugins.get("CommentsRepository"),Y=d.data.get({showSuggestionHighlights:!0}),F=d.data.get(),H=h.getSuggestions({skipNotAttached:!0,skipEmpty:!0,toJSON:!0}),G=M.getCommentThreads({skipNotAttached:!0,skipEmpty:!0,toJSON:!0});g("update",{content:Y,rawContent:F,suggestions:B(H),commentThreads:G})},A=async d=>{if(!a.value)return;const h=a.value;d?h.execute("acceptAllSuggestions"):h.execute("discardAllSuggestions");const M=h.plugins.get("TrackChanges"),Y=h.plugins.get("CommentsRepository"),F=h.data.get(),H=M.getSuggestions({skipNotAttached:!0,skipEmpty:!0,toJSON:!0}),G=Y.getCommentThreads({skipNotAttached:!0,skipEmpty:!0,toJSON:!0});g("update",{content:F,rawContent:F,suggestions:B(H),commentThreads:G})},B=(d,h=!1)=>Array.isArray(d)?d.map(M=>({...M,createdAt:h?m.convertEpochToDateTime(M.createdAt):m.dateToEpoch(M.createdAt)})):d;return k({apply:T,acceptChanges(){A(!0)},discardChanges(){A(!1)}}),(d,h)=>(c(),p("div",le,[vt(t("div",ce,[o(s(E),{"initial-data":s(C)||s(y),config:b.value},null,8,["initial-data","config"])],512),[[ht,d.editting]]),vt(t("div",re,[nt(d.$slots,"content",{content:s(y)},()=>[t("div",{innerHTML:s(y)},null,8,de)],!0)],512),[[ht,!d.editting]])]))}});const ue=J(me,[["__scopeId","data-v-1e1bd178"]]),pe=["onClick"],_e=["alt"],ve=R({__name:"PromotionMedia",props:{items:{default:()=>[]}},setup(v){const k=v,{items:g}=q(k),e=S(()=>g.value.map(C=>({src:C.uploadedUrl||C.url,thumb:C.uploadedUrl||C.url}))),m=N(),y=N();return Rt(()=>{y.value=ne(m.value,{plugins:[se],dynamic:!0,dynamicEl:e.value})}),(C,_)=>{const w=K;return c(),p("div",{ref_key:"mediaElement",ref:m,class:dt(["Media"])},[(c(!0),p(W,null,Q(s(g),($,i)=>(c(),p("div",{key:`media-item-${i}`,class:"Media-item",onClick:x(a=>y.value.openGallery(i),["prevent"])},[t("div",{class:"Media-thumb",style:gt({backgroundImage:`url(${$.uploadedUrl||$.url})`}),alt:$.name},null,12,_e),t("button",{type:"button",class:"Media-button",onClick:_[0]||(_[0]=x(()=>{},["stop"]))},[o(w,{symbol:"trash",class:"text-error"})])],8,pe))),128))],512)}}});const he=v=>(Ct("data-v-b7ed22c5"),v=v(),bt(),v),fe={class:"Timeline"},ge={key:0,class:"p-4 text-center"},ye=he(()=>t("em",null,"No logs yet.",-1)),Ce=[ye],be={key:1,class:"Timeline-body"},xe={key:0},ke={class:"Timeline-icon"},$e={class:"Timeline-contentTop"},we={class:"Timeline-title"},Te={class:"font-weight-semi-bold"},Pe={class:"Timeline-time"},Ae={class:"Timeline-switchMode"},De={class:"Timeline-contentBody"},Se=["innerHTML"],Me=R({__name:"PromotionLogs",props:{promotion:{}},setup(v){const k=v,{promotion:g}=q(k),{currentCustomerName:e}=st(yt()),m=kt(),{comments:y}=st(m),{fetchCommentsAsync:C}=m,_=S(()=>{const i=[];return y.value.sort((a,b)=>Number(a.createdAt)<Number(b.createdAt)?1:-1).forEach(a=>{const b=i[i.length-1];(!b||Z(Number(a.createdAt)*1e3).format("L")!==Z(Number(b.createdAt)*1e3).format("L"))&&i.push({...a,id:Ot(),isForDate:!0,isPublic:!0}),i.push(a)}),i}),w=i=>{switch(i){case D.Comment:return"comment-bubble";case D.Approve:return"check-big";case D.Reject:return"clear";case D.Create:case D.Modify:default:return"edit-pdf-50"}},$=i=>{switch(i.commentType){case D.Comment:return"left a comment.";case D.Approve:return"approved this post.";case D.RevokeApprove:return"revoke the approval.";case D.RevokeReject:return"revoke the rejection.";case D.Reject:return"rejected this post.";case D.Modify:return"modified the text.";case D.Create:return"created this post."}};return jt(g,i=>{i.id&&C(g.value.id,V.CommentFinancialPromotion)},{immediate:!0}),(i,a)=>{const b=K,E=Bt,j=mt;return c(),p("div",fe,[_.value.length?(c(),p("div",be,[(c(!0),p(W,null,Q(_.value,T=>(c(),p("div",{key:`timeline-log-${T.id}`,class:dt([`Timeline-${T.isForDate?"heading":"item"}`])},[T.isForDate?(c(),p("strong",xe,I(s(Z)(Number(T.createdAt)*1e3).calendar({sameDay:"[Today]",lastDay:"[Yesterday]",sameElse:"MMMM D, YYYY"})),1)):(c(),p(W,{key:1},[t("div",ke,[o(b,{size:"18",symbol:w(T.commentType)},null,8,["symbol"])]),o(j,{class:"Timeline-content"},{default:l(()=>[t("div",$e,[t("div",we,[o(E,{text:s(e),rounded:"full",size:"small"},{text:l(()=>[t("span",Te,I(s(e)),1),P(" "+I($(T)),1)]),_:2},1032,["text"])]),t("div",Pe,I(s(Z)(Number(T.createdAt)*1e3).fromNow()),1),t("button",Ae,[T.isPublic?(c(),O(b,{key:0,size:"20",symbol:"earth-1-33",class:"text-tertiary"})):(c(),O(b,{key:1,size:"20",symbol:"user-multiple-circle-28",class:"text-primary"})),o(b,{size:"12",symbol:"arrow-down-3-79"})])]),t("div",De,[t("div",{innerHTML:T.commentText,class:"is-text-tight"},null,8,Se)])]),_:2},1024)],64))],2))),128))])):(c(),p("div",ge,Ce))])}}});const Ie=J(Me,[["__scopeId","data-v-b7ed22c5"]]),Ee={class:"PromotionCommentInput"},ze={class:"PromotionCommentInput-buttons"},Le=R({__name:"PromotionCommentInput",emits:["submit"],setup(v,{emit:k}){const g=ct.resolve(rt.name),e=N(""),m=S(()=>!g.stripHTMLTags(e.value).trim()),y=C=>{k("submit",e.value,C),e.value=""};return(C,_)=>{const w=Ft,$=K,i=ut;return c(),p("div",Ee,[o(w,{value:e.value,class:"PromotionCommentInput-editor",tools:[],"content-style":{height:"30px"},placeholder:"Add Comment",onChange:_[0]||(_[0]=a=>e.value=a.html)},null,8,["value"]),t("div",ze,[o(i,{disabled:m.value,"theme-color":"tertiary",class:"text-primary font-weight-semi-bold",onClick:_[1]||(_[1]=x(a=>y(!0),["prevent"]))},{default:l(()=>[o($,{size:"15",symbol:"earth-1-33"}),P(" Public ")]),_:1},8,["disabled"]),o(i,{disabled:m.value,"theme-color":"primary",onClick:_[2]||(_[2]=x(a=>y(!1),["prevent"]))},{default:l(()=>[o($,{size:"15",symbol:"user-multiple-circle-28"}),P(" Internal ")]),_:1},8,["disabled"])])])}}});const Ne=J(Le,[["__scopeId","data-v-9178fc55"]]),tt=v=>(Ct("data-v-37408c6a"),v=v(),bt(),v),Re={class:"PromotionDisclosure"},je=tt(()=>t("img",{src:ae,alt:"PDF",width:"23",height:"24"},null,-1)),Be=tt(()=>t("a",{href:"javascript:;",class:"PromotionDisclosure-link"}," View Disclosure ",-1)),Oe={class:"row align-items-center"},Fe=tt(()=>t("div",{class:"col-lg-9 gx-3 PromotionDisclosure-text"},[P(" I hereby confirm that all "),t("a",{href:"javascript:;"},"Disclosures"),P(" are included within the content, while maintaining adequate prominence of no less than the prominence afforded to the content itself. ")],-1)),He={class:"col-lg-3"},Ue={key:1,class:"font-size-sm"},Ve=tt(()=>t("br",null,null,-1)),We=tt(()=>t("br",null,null,-1)),qe={class:"font-size-lg text-primary mb-3"},Je=["data-index","innerHTML"],Ke=R({__name:"PromotionDisclosures",props:{promotion:{},disclosures:{default:()=>({})}},emits:["confirm"],setup(v,{emit:k}){const g=v,e=ct.resolve(rt.name),{promotion:m,disclosures:y}=q(g),C=S(()=>{var i;return((i=m.value.editorContent)==null?void 0:i.content)||""}),_=i=>{const a=e.stripHTMLTags(C.value).trim();return!!(i.id==="timePeriodDisclosure"||i.id==="affiliateDisclosure"&&(e.isWordExist(a,"providers")||e.isWordExist(a,"provider"))||i.id==="taxDisclosure"&&(e.isWordExist(a,"taxes")||e.isWordExist(a,"tax")))},w=S(()=>V.disclosureTypes.map(i=>({...i,content:y.value[`${i.id}ConfirmedText`]||[]})).filter(i=>Ht.isEmpty(i.content)?!1:_(i))),$=N(!1);return(i,a)=>{const b=K,E=ut,j=mt,T=ft;return c(),p("div",Re,[o(j,{class:"PromotionDisclosure-button",onClick:a[1]||(a[1]=x(A=>$.value=!0,["prevent"]))},{default:l(()=>[je,Be,o(E,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"Download",class:"PromotionDisclosure-download",onClick:a[0]||(a[0]=x(()=>{},["stop"]))},{default:l(()=>[o(b,{symbol:"download-box-2-19",size:"18",class:"text-primary"})]),_:1})]),_:1}),t("div",Oe,[Fe,t("div",He,[s(m).isDisclosureConfirmed?(c(),p("span",Ue,[P(" Jenny Berns"),Ve,P(" 17 November 2023"),We,P(" 08:09 AM ")])):(c(),O(E,{key:0,"theme-color":"primary",class:"w-100",onClick:a[2]||(a[2]=x(A=>k("confirm"),["prevent"]))},{default:l(()=>[P(" Confirm ")]),_:1}))])]),o(T,{modelValue:$.value,"onUpdate:modelValue":a[4]||(a[4]=A=>$.value=A),"fixed-height":"",width:"1300"},{"before-close":l(()=>[o(E,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"Download",onClick:a[3]||(a[3]=x(()=>{},["stop"]))},{default:l(()=>[o(b,{symbol:"download-box-2-19",size:"18",class:"text-primary"})]),_:1})]),default:l(()=>[(c(!0),p(W,null,Q(w.value,A=>(c(),p("div",{key:`disclosure-${A.id}`,class:"mb-5"},[t("h4",qe,I(A.title),1),(c(!0),p(W,null,Q(A.content,(B,d)=>(c(),p("div",{key:`disclosure-${A.id}-${d}`,"data-index":d,innerHTML:B},null,8,Je))),128))]))),128))]),_:1},8,["modelValue"])])}}});const Ye=J(Ke,[["__scopeId","data-v-37408c6a"]]),Ge=R({__name:"PromotionDetails",setup(v){const k=Ut();return(g,e)=>{const m=Vt;return c(),O(m,{class:"PromotionDetails",items:[{id:"details-text",title:"Text",content:"",active:!0},{id:"details-media",title:"Photos & Videos",content:""},{id:"details-attachments",title:"Attachments",content:""}]},xt({_:2},[s(k)["text-content"]?{name:"content-details-text",fn:l(()=>[nt(g.$slots,"text-content",{},void 0,!0)]),key:"0"}:void 0,s(k)["media-content"]?{name:"content-details-media",fn:l(()=>[nt(g.$slots,"media-content",{},void 0,!0)]),key:"1"}:void 0,s(k)["attachments-content"]?{name:"content-details-attachments",fn:l(()=>[nt(g.$slots,"attachments-content",{},void 0,!0)]),key:"2"}:void 0]),1024)}}});const Xe=J(Ge,[["__scopeId","data-v-57a80a47"]]),Ze={class:"Attachment-block"},Qe=["alt"],to=Wt('<div class="Attachment-info" data-v-9351fa49><span class="Attachment-name" data-v-9351fa49><img src="'+ie+'" width="24" height="24" data-v-9351fa49><a href="#" class="is-truncated" title="filenamesdfdfsdfsdf.pdf" data-v-9351fa49> filenamesdfdfsdfsdf.pdf </a></span><span class="Attachment-date is-truncated" data-v-9351fa49> Sep 19, 2020 | 10:52 AM </span></div>',1),eo={class:"Attachment-buttons"},oo=R({__name:"PromotionAttachments",props:{items:{default:()=>[]}},setup(v){const k=v,{items:g}=q(k),e=N();return(m,y)=>{const C=K;return c(),p("div",{ref_key:"mediaElement",ref:e,class:dt(["Attachment"])},[(c(!0),p(W,null,Q(s(g),(_,w)=>(c(),p("div",{key:`attachment-item-${w}`,class:"Attachment-item"},[t("div",Ze,[t("div",{class:"Attachment-thumb",style:gt({backgroundImage:`url(${_.uploadedUrl||_.url})`}),alt:_.name},null,12,Qe),to,t("div",eo,[t("button",{type:"button",class:"Attachment-button",onClick:y[0]||(y[0]=x(()=>{},["stop"]))},[o(C,{symbol:"eye",class:"text-primary"})]),t("button",{type:"button",class:"Attachment-button",onClick:y[1]||(y[1]=x(()=>{},["stop"]))},[o(C,{symbol:"trash",class:"text-error"})])])])]))),128))],512)}}});const no=J(oo,[["__scopeId","data-v-9351fa49"]]),so={class:"row"},ao={class:"col-lg-2"},io=t("label",null,"Source",-1),lo={class:"col-lg-2"},co=t("label",null,"Content Status",-1),ro={class:"col-lg-2"},mo=t("label",null,"Content Owner",-1),uo={class:"col-lg-2"},po=t("label",null,"Moderator",-1),_o=t("div",{class:"col-lg-2"},[t("label",null,"Approval Type"),t("span",null,"Existing Post")],-1),vo={class:"col-lg-2"},ho=t("label",null,"Approval Required Date",-1),fo={class:"col-lg-2"},go=t("label",null,"Financial Promotion Type",-1),yo={class:"col-lg-2"},Co=t("label",null,"Provider Name",-1),bo={class:"col-lg-2"},xo=t("label",null,"Product Type",-1),ko={class:"is-truncated d-inline-block w-100"},$o={class:"col-lg-2"},wo=t("label",null,"Remuneration Method",-1),To={class:"col-lg-2"},Po={class:"d-flex align-items-center gap-2"},Ao={class:"col-lg-2"},Do={class:"d-flex align-items-center gap-2"},So={key:1,class:"row is-stretched-content"},Mo={class:"col-lg-6"},Io={class:"row"},Eo={class:"col-md-6"},zo=["href"],Lo={class:"col-md-6 text-right"},No={key:0,class:"d-inline-flex gap-2"},Ro=t("strong",{class:"font-weight-semi-bold"},"Cancel",-1),jo=t("strong",null,"Done",-1),Bo={class:"d-flex align-items-center gap-2 px-1"},Oo=t("strong",null,"Edit to Approve",-1),Fo=["innerHTML"],Ho={key:0,class:"RejectApproveButtons"},Uo={class:"col-lg-6 is-vertical-content gap-4"},tn=R({__name:"ViewPromotionModal",props:{promotion:{default:()=>({})},loading:{type:Boolean}},emits:["edit"],setup(v,{emit:k}){const g=v,{promotion:e}=q(g),m=N(!1),y=te(),{getMediaMarketingOutlet:C}=y,_=ee(),{saveOrUpdateFinancialPromotionAsync:w}=_,$=kt(),{addOrEditCommentAsync:i}=$,a=S(()=>C(`${e.value.mediaOutlet}`,!1)),{currentCustomer:b}=st(yt()),E=Gt(),{staffs:j}=st(E),{fetchStaffsAsync:T}=E;T();const A=S(()=>b.value.id===e.value.moderator),B=S(()=>j.value.map(n=>({...n,avatar:`https://api.dicebear.com/7.x/avataaars/svg?backgroundColor=b6e3f4,c0aede,d1d4f9&seed=${n.name}`}))),d=N(),h=N(!1),M=()=>{d.value.apply(),h.value=!1},Y=()=>{lt({content:"Do you wish to reject the text change(s)?",confirmButtonText:"Confirm",onConfirm(){d.value.discardChanges()}})},F=()=>{lt({content:"Do you wish to approve the text change(s)?",confirmButtonText:"Confirm & Approve",onConfirm(){d.value.acceptChanges()}})},H=n=>{lt({title:"Confirm",content:n?"Do you wish to approve this post?":"Do you wish to reject this post?",confirmButtonText:n?"Confirm & Approve":"Confirm & Reject",confirmButtonThemeColor:n?"primary":"error",async onConfirm(){try{m.value=!0,await w({...e.value,approvalStatus:n?ot.Approved:ot.Rejected}),Jt({type:Kt.SUCCESS,content:n?"Post Approved.":"Post Rejected."}),m.value=!1}catch{}}})},G=S(()=>{var f;const{editorContent:n,contentUrl:u,content:r}=e.value;return u&&(r!=null&&r.textContent)&&!(n!=null&&n.content)?((f=r.textContent)==null?void 0:f.text)||"":(n==null?void 0:n.content)||""}),$t=S(()=>{var f;const{editorContent:n,contentUrl:u,content:r}=e.value;return u&&(r!=null&&r.textContent)&&!(n!=null&&n.rawContent)?((f=r.textContent)==null?void 0:f.text)||"":(n==null?void 0:n.rawContent)||""}),pt=S(()=>{const{media:n,contentUrl:u,content:r}=e.value;return u&&(r!=null&&r.images)?r.images:n==null?void 0:n.filter(f=>V.ImageExtensions.includes(f.extension??""))}),wt=S(()=>{const{media:n,contentUrl:u,content:r}=e.value;return u&&(r!=null&&r.documents)?r.documents:n==null?void 0:n.filter(f=>V.DocumentExtensions.includes(f.extension??""))}),Tt=async n=>{n={editorContent:{content:n.content,rawContent:n.rawContent,suggestions:n.suggestions,commentThreads:n.commentThreads}};try{m.value=!0,await w({...e.value,...n,isDisclosureConfirmed:!1}),await i({contentId:e.value.id,contentType:V.CommentFinancialPromotion,commentType:D.Modify,isPublic:!0}),m.value=!1}catch{}},Pt=(n,u)=>{i({contentId:e.value.id,contentType:V.CommentFinancialPromotion,commentType:D.Comment,commentText:n,isPublic:u})},At=async()=>{m.value=!0,await w({...e.value,isDisclosureConfirmed:!0}),m.value=!1},Dt=n=>{const u=["crypto"];let r=n;for(const f of u){const et=new RegExp(f,"gi");r=r.replace(et,`<span class="warn">${f}</span>`)}return r};return(n,u)=>{const r=K,f=ut,et=Xt,St=Zt,_t=Qt,Mt=Yt,X=mt,It=Lt,Et=ft;return c(),O(Et,{width:"1300","fixed-height":"",class:"ViewPromotionModal"},{title:l(()=>[P(I(n.$t("marketingFinancialPage-previewFinancialPromotion"))+" ",1),o(f,{type:"button","fill-mode":"flat",shape:"square",size:"small",onClick:u[0]||(u[0]=x(L=>k("edit",s(e)),["prevent"]))},{default:l(()=>[o(r,{symbol:"edit-pen",size:"20"})]),_:1})]),default:l(()=>[o(It,{loading:n.loading||m.value,class:"is-vertical-content"},{default:l(()=>[s(e)?(c(),O(X,{key:0,class:"PromotionInfo"},{default:l(()=>{var L,z;return[t("div",so,[t("div",ao,[io,o(et,{id:(L=a.value)==null?void 0:L.platform,"icon-size":25,text:(z=a.value)==null?void 0:z.name,class:"gap-1"},null,8,["id","text"])]),t("div",lo,[co,o(St,{live:s(e).isLive},null,8,["live"])]),t("div",ro,[mo,o(_t,{id:s(e).owner,size:"small","text-only":"","text-truncated":""},null,8,["id"])]),t("div",uo,[po,o(_t,{id:s(e).moderator,size:"small",rounded:"full","text-truncated":""},null,8,["id"])]),_o,t("div",vo,[ho,t("span",null,I(s(Z)((s(e).createdAt||0)*1e3).add(s(e).approvalDays||0,"days").format("LL")),1)]),t("div",fo,[go,t("span",null,I(s(e).type),1)]),t("div",yo,[Co,t("span",null,I(s(e).provider),1)]),t("div",bo,[xo,o(Mt,{items:s(e).productType,"popup-title":"Product Type"},{"visible-items":l(({items:U})=>[t("span",ko,I((U||[]).join(",")),1)]),_:1},8,["items"])]),t("div",$o,[wo,t("span",null,I(s(e).remunerationMethod),1)]),t("div",To,[o(f,{"theme-color":"error",class:"w-100",disabled:!A.value||s(e).approvalStatus!==s(ot).Pending,onClick:u[1]||(u[1]=x(U=>H(!1),["prevent"]))},{default:l(()=>[t("div",Po,[o(r,{symbol:"clear",size:"12"}),P(" Reject ")])]),_:1},8,["disabled"])]),t("div",Ao,[o(f,{"theme-color":"success",class:"w-100",disabled:!A.value||s(e).approvalStatus!==s(ot).Pending,onClick:u[2]||(u[2]=x(U=>H(!0),["prevent"]))},{default:l(()=>[t("div",Do,[o(r,{symbol:"check-big",size:"12"}),P(" Approve ")])]),_:1},8,["disabled"])])])]}),_:1})):it("",!0),s(e)?(c(),p("div",So,[t("div",Mo,[o(X,{class:"mb-0 h-100"},{head:l(()=>{var L;return[t("div",Io,[t("div",Eo,[t("a",{href:s(e).contentUrl??"#",target:"_blank"},[o(et,{id:(L=a.value)==null?void 0:L.platform,"icon-size":30,class:"gap-1"},{text:l(()=>{var z;return[t("b",null,I((z=a.value)==null?void 0:z.name),1),P(" (Source) ")]}),_:1},8,["id"])],8,zo)]),t("div",Lo,[h.value?(c(),p("div",No,[o(f,{"theme-color":"primary",size:"small","fill-mode":"flat",onClick:u[3]||(u[3]=x(z=>h.value=!1,["prevent"]))},{default:l(()=>[Ro]),_:1}),o(f,{"theme-color":"primary",size:"small",onClick:x(M,["prevent"])},{default:l(()=>[jo]),_:1},8,["onClick"])])):(c(),O(f,{key:1,"theme-color":"warning",size:"small",onClick:u[4]||(u[4]=x(z=>h.value=!0,["prevent"]))},{default:l(()=>[t("div",Bo,[o(r,{symbol:"pencil-7",size:"12"}),Oo])]),_:1}))])])]}),default:l(()=>[o(Xe,null,xt({"attachments-content":l(()=>[o(no,{items:wt.value},null,8,["items"])]),_:2},[s(e).editorContent?{name:"text-content",fn:l(()=>{var L,z,U;return[o(ue,{ref_key:"contentElement",ref:d,editting:h.value,"user-id":s(b).id,users:B.value,content:String(G.value),"raw-content":String($t.value),suggestions:(L=s(e).editorContent)==null?void 0:L.suggestions,"comment-threads":(z=s(e).editorContent)==null?void 0:z.commentThreads,onUpdate:Tt},{content:l(({content:zt})=>[t("div",{innerHTML:Dt(zt)},null,8,Fo)]),_:1},8,["editting","user-id","users","content","raw-content","suggestions","comment-threads"]),s(qt)((U=s(e).editorContent)==null?void 0:U.suggestions)?it("",!0):(c(),p("div",Ho,[o(f,{size:"small","fill-mode":"outline","theme-color":"error",onClick:x(Y,["prevent"])},{default:l(()=>[P(" Reject All Text Changes ")]),_:1},8,["onClick"]),o(f,{size:"small","theme-color":"success",onClick:x(F,["prevent"])},{default:l(()=>[P(" Approve All Text Changes ")]),_:1},8,["onClick"])]))]}),key:"0"}:void 0,pt.value?{name:"media-content",fn:l(()=>[o(ve,{items:pt.value},null,8,["items"])]),key:"1"}:void 0]),1024)]),_:1})]),t("div",Uo,[o(X,{title:"Disclosures",class:"mb-0"},{default:l(()=>[o(Ye,{promotion:s(e),disclosures:s(b).disclosure,onConfirm:At},null,8,["promotion","disclosures"])]),_:1}),o(X,{title:"Log History",class:"mb-0 is-stretched-content LogHistory","body-class":"is-scrollable-y"},{default:l(()=>[o(Ie,{promotion:s(e)},null,8,["promotion"])]),_:1}),o(X,{class:"mb-0"},{default:l(()=>[o(Ne,{onSubmit:Pt})]),_:1})])])):it("",!0)]),_:1},8,["loading"])]),_:1})}}});export{tn as default};