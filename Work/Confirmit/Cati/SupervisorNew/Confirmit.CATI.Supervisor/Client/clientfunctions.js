var LoadingDivMessage = "Loading...";
var LoadingDivFonts = "Verdana, Geneva, Arial, Helvetica, sans-serif";
var sEmptyFrameSrc = "javascript:document.open( );document.close( );";
var MAX_INT = 2147483647;

//-------------------------------------------------------------------
function getDocumentBase() 
{
    var oBaseColl = document.getElementsByTagName( "BASE" );
    if( oBaseColl && oBaseColl.length )
    {
        return( oBaseColl[ 0 ].href );
	} 
	else 
	{
		return( "" );
    } 
}

//-------------------------------------------------------------------
function StrToInt(strg, error)
{
	var value 
	try
	{
		value = Number(strg);
	}
	catch(e)
	{ 
		throw error;
	}
	
	if(isNaN(value))
		throw error;
		
	return value;
}

//-------------------------------------------------------------------
function CheckIntValue(strValue)
{
	var nValue = StrToInt(strValue, "Wrong Value");

	if(nValue < 0)
		throw "Value can not be less than 0";
}

//-------------------------------------------------------------------
function CheckIntMinMax(strMin, strMax)
{
	var nMin = StrToInt(strMin, "Wrong Min value");
	var nMax = StrToInt(strMax, "Wrong Max value");

	if(nMin < 0)
		throw "Min value can not be less than 0";

	if( nMin > nMax )
	{
		throw "Min value can not be greater than Max value";
	}
}

//-------------------------------------------------------------------
function getWindowsFileNameFromString( sPath )
{
	var re = /[\%\&\\/\:\*?\"\<\>\|]/ig;
	var sRes = sPath.replace( re, "" );
	
	while(sRes.indexOf("..") > -1)
	{
		sRes = replace(sRes, "..", ".");
	}
	
	return( sRes );
}

//-------------------------------------------------------------------
function getSingleSelectDefaultValue( oSel )
{
	for( var i = 0; i < oSel.options.length; i++ )
	{
		if( oSel.options[ i ].defaultSelected )
		{
			return( oSel.options[ i ].value );
		}
	}
	return( null );
}

//-------------------------------------------------------------------
function getRadioDefaultValue( el )
{
	var list = el.elements[ el.name ];
	if( list.length )
	{
		for( var i = 0; i < list.length; i++ )
		{
			var item = list[ i ];
			if( item.defaultChecked )
			{
				return( item.defaultValue );
			}
		}
	}
	else
	{
		return( list.defaultChecked ? list.defaultValue : null );
	}
	
	return( null );
}

//-------------------------------------------------------------------
function isFormChanged( form )
{
	for( var i = 0; i < form.elements.length; i++ )
	{
		var el = form.elements[ i ];
		switch( el.tagName.toLowerCase() )
		{
			case "select":
				if( getSingleSelectDefaultValue( el ) != el.value )
				{
					return( true );
				}
				break;
			case "input":
				switch( el.type.toLowerCase() )
				{
					case "radio":
						if( getRadioDefaultValue( el ) != getRadio( form, el.name ) )
						{
							return( true );
						}
						break;
					case "checkbox":
						if( el.defaultChecked != el.checked )
						{
							return( true );
						}
						break;
					default:
						if( el.value != el.defaultValue )
						{
							return( true );
						}
						break;
				}
				break;
			default:
				if( el.value != el.defaultValue )
				{
					return( true );
				}
				break;
		}
	}
	return( false );
}

//-------------------------------------------------------------------
function displayError( err )
{
	if( typeof( err ) == "object" )
	{
		alert( err.description );
	}
	else
	{
		alert( err );
	}
}

//-------------------------------------------------------------------
function checkEmptyFrame( oFrame )
{
	if( oFrame.src == sEmptyFrameSrc || 
		oFrame.document.location.href == 
		oFrame.parent.document.location.href )
	{
		return ( true );
	}
	
	return( false );
}

//-------------------------------------------------------------------
function showLoadingDivOnObject( oObject, bShow, bShowBorder, iFontSize )
{
	if( typeof( iFontSize ) == "undefined" )
	{
		iFontSize = 14;
	}
	if( bShow )
	{
		var nDivId = oObject.uniqueID + "_LoadingDiv";
		var strg;
		var oDiv;
		oDiv = document.getElementById( nDivId );
		if( oDiv )
		{
			oDiv.removeNode( true );
		}
		oDiv = document.createElement( "div" );
		var oDivStyle = oDiv.style;
		oDivStyle.position = "absolute";
		if( bShowBorder )
		{
			oDivStyle.border = "thin inset";
		}
		oDiv.id = nDivId;
		
		var oCoords = getObjectCoords( oObject );
		
		oDivStyle.left = oCoords.left;
		oDivStyle.top = oCoords.top;
		
		oDiv.width = oObject.clientWidth;
		oDiv.height = oObject.clientHeight;
		oDivStyle.zIndex = parseInt( oObject.style.zIndex ) + 1;
		strg = "<table style=\"background-color:white;\" width=" + oDiv.width + " height=" + oDiv.height + "><tr><td valign=middle align=center width=100% height=100% style=\"font-size: " + iFontSize + "px; font-family : " + LoadingDivFonts + ";\">";
		if (oDiv.width > 110 && oDiv.height > 150) 
		{
			strg = strg + "<img src=images/clientfunctions/progress.gif border=0 alt=\"\"><br>";
		}
		strg = strg + LoadingDivMessage;
		strg = strg + "</td></tr></table>";
		oDiv.innerHTML = strg;
		oObject.appendChild( oDiv );

		return( oDiv );
	}
	else
	{
		var oDiv = document.getElementById( oObject.uniqueID + "_LoadingDiv" );
		if( oDiv )
		{
			oObject.removeChild( oDiv, true );
		}
	}
}

//-------------------------------------------------------------------
function getObjectCoords( oObject )
{
	var oCoords = new Object( );
	var left = 0;
	var top = 0;
	
	var oParent = oObject;
	
	while( oParent )
	{
		left += oParent.offsetLeft;
		top += oParent.offsetTop;
		oParent = oParent.offsetParent;
	}
	
	oCoords.left = left;
	oCoords.top = top;
	
	return( oCoords );
}

//-------------------------------------------------------------------
function disableObject( oElement, bDisable )
{
	if( !oElement )
	{
		return;
	}
	if( oElement.disabled != bDisable )
	{
		var i;
		if( oElement.tagName.toLowerCase() != "select" )
		{
			for( i = 0; i < oElement.children.length; i++ )
			{
				disableObject( oElement.children[ i ], bDisable );
			}
		}
		oElement.disabled = bDisable;
	}
}

//-------------------------------------------------------------------
function SKEBlockObject( objectToBlock, val )
{
	//this function temporary locks the page while opening a node
	if( !document.SKEDynamicBlockObjectDiv )
	{
		var html_text = "<DIV id=\"SKEDynamicBlockObjectDiv\" onClick=\"event.cancelBubble=true;return(false);\" style=\"top:0;left:0;width:100%;height:100%;position:absolute;z-index:1000;display:none;\">";
		html_text += "<img src=\"images/clientfunctions/null.gif\" width=\"100%\" height=\"100%\" border=0 GALLERYIMG=\"no\">"
		html_text += "</DIV>";
		document.body.insertAdjacentHTML("afterBegin", html_text);
		document.SKEDynamicBlockObjectDiv = document.getElementById( "SKEDynamicBlockObjectDiv" );
	}
	if (val)
	{
		document.SKEDynamicBlockObjectDiv.style.width  = objectToBlock.scrollWidth;
		document.SKEDynamicBlockObjectDiv.style.height = objectToBlock.scrollHeight;
	}
	document.SKEDynamicBlockObjectDiv.style.display = val ? "block" : "none";
}

//-------------------------------------------------------------------
function ArrayRemoveItemByValue( value )
{
	var arr = new Array( );
	for( var i = 0; i < this.length; i++ )
	{
		if( this[ i ] != value )
		{
			arr[ arr.length ] = this[ i ];
		}
	}
	//this = arr;
	
	return( arr );
}

//-------------------------------------------------------------------
function decodeEncodedString(strg)
{
	var tmp = strg;

	while (1)
	{
		var re = /\&\#\x[\dA-Fa-f]*\;/i;
		var arr = tmp.match(re);
		if (!arr)
		{
			break;
		}
		var val = arr[0];
		val = val.substring(3, val.length - 1);
		val = (val.length == 2) ? "%" + val : "%u" + val;
		
		tmp = tmp.substring(0, arr.index) + val + tmp.substring(arr.lastIndex, tmp.length);
	}
	tmp = unescape(tmp);

	return(tmp);
}

//-------------------------------------------------------------------
function checkInt(strg)
{
	try
	{
		return(parseInt(trim(strg))+"" == trim(strg));
	}
	catch(e)
	{ 
		return( false );
	}
}

//-------------------------------------------------------------------
function checkFloat(strg)
{
	try
	{
		return(parseFloat(trim(strg))+"" == trim(strg));
	}
	catch(e)
	{ 
		return( false );
	}
}

//-------------------------------------------------------------------
function checkDateTime(day, month, year, hours, minutes, seconds)
{
	var prov = true;
	var isVis = false;
	if ( isNaN(day+month+year+hours+minutes+seconds) )
		prov = false;
	if (prov)
	{
		if ( day<0 || month<0 || year<0 || hours<0 || minutes<0 || seconds<0 )
			prov = false;
	}
	if (prov)
	{
		if ( (year % 4 == 0) && ( (year % 100 != 0) || (year % 400 == 0) ) )
			isVis = true;
		switch (month)
		{
			case 1:
			case 3:
			case 5:
			case 7:
			case 8:
			case 10:
			case 12:
				if (day>31)
					prov = false;
				break;
			case 2:
				if ( ( day>28 ) && !( day==29 && isVis ) )
					prov = false;
				break;
			case 4:
			case 6:
			case 9:
			case 11:
				if (day>30)
					prov = false;
				break;
			default:
				prov = false;
				break;
		}
	}
	
	if (hours>23)
		prov = false;
	if (minutes>59)
		prov = false;
	if (seconds>59)
		prov = false;
	
	return (prov);
}

//-------------------------------------------------------------------
function AddHandler(obj, evnt, handler)
{
	if (!obj)
		return (false);
	var strg="";
	eval ("var prov = obj."+evnt);
	if (prov)
	{
		strg = prov.toString();
		strg = strg.substr(0, strg.lastIndexOf("}"))+";\n"+handler+"\n}";
	}
	else
		eval("strg = function() {"+handler+"}");
	eval("obj."+evnt+" = "+strg)
}

//-------------------------------------------------------------------
function AddToBodyClick(strgToAdd)
{
	AddHandler(document.body, "onclick", strgToAdd);
}

//-------------------------------------------------------------------
function findInParents(obj, tagName)
{
	var par = obj;
	var el;
	while (par.tagName!="BODY")
	{
		par = par.parentElement;
		if (par.tagName.toLowerCase()==tagName.toLowerCase())
			return (par);
	}
	return (null);
}

//-------------------------------------------------------------------
function trim(strg)
{
	return(rtrim(ltrim(strg)));
}

//-------------------------------------------------------------------
function ltrim(strg)
{
	var temp = strg;
	while (temp.charAt(0)==" ")
	{
		temp = temp.substring(1, temp.length)
	}
	return (temp);
}

//-------------------------------------------------------------------
function rtrim(strg)
{
	var temp = strg;
	while (temp.charAt(temp.length-1)==" ")
	{
		temp = temp.substring(0, temp.length-1)
	}
	return (temp);
}


//-------------------------------------------------------------------
function Move_UP(f, s) 
{
	var t=findInForm(f,s);
	if (t>-1) 
	{
		if( !f.elements[t].multiple )
		{
			var sel=f.elements[t].options.selectedIndex;
			if (sel>0) 
			{
				var strg=f.elements[t].options[sel-1].value;
				var strg1=f.elements[t].options[sel-1].text;
				var st = f.elements[t].options[sel-1].style.cssText;
	
				f.elements[t].options[sel-1].value=f.elements[t].options[sel].value;
				f.elements[t].options[sel-1].text=f.elements[t].options[sel].text;
				f.elements[t].options[sel-1].style.cssText = f.elements[t].options[sel].style.cssText;
	
				f.elements[t].options[sel].value=strg;
				f.elements[t].options[sel].text=strg1;
				f.elements[t].options[sel].style.cssText = st;
	
				f.elements[t].options.selectedIndex=sel-1;
				return (1);
			}
		}else
		{
			var el = f.elements[t];
			var all_grp = true;
			var res = false;
			
			for( var i = 0; i < el.options.length; i++ )
			{
				if( ( !el.options[i].selected ) || ( el.options[i].alreadymoved ) ) { continue; } 
				
				if( el.options[i].parentElement.tagName != "OPTGROUP" )
				{//root
					for( var j = 0; j < el.childNodes.length; j++ ) 
					{
						if( ( el.childNodes[j] == el.options[i] ) && ( !el.childNodes[j-1] ) ) 
						{
							return res;
						}
						
						if( ( el.childNodes[j] == el.options[i] ) && ( el.childNodes[j-1] ) ) 
						{
							res = true;
							el.childNodes[j].alreadymoved = true;
							el.childNodes[j].swapNode( el.childNodes[j-1] );
							
							break;
						}
					}	
					continue;
				}
					
				var grp = el.options[i].parentElement;
				var grp_i;
				for( var j = 0; j < grp.childNodes.length; j++ )
				{
					if( !grp.childNodes[j].selected ) 
					{ 
						all_grp = false;
					}
					if( grp.childNodes[j] == el.options[i] ) 
					{
						grp_i = j;
					}
				}
				
				if( !all_grp ) //movement inside group
				{
										
					if( ( !grp.childNodes[ grp_i - 1 ] ) || ( grp.childNodes[ grp_i - 1 ].alreadymoved ) )
					{
						return res;
						//el.options[i].alreadymoved = true;
						//continue;
					}
					
					var sel = i;
					var sel2 = i - 1;
					res = true;
					var strg = el.options[sel2].value;
					var strg1 = el.options[sel2].text;
					var strg2 = el.options[sel2].type;
					var st = el.options[sel2].style.cssText;
		
					el.options[sel2].value = el.options[sel].value;
					el.options[sel2].text = el.options[sel].text;
					el.options[sel2].type = el.options[sel].type;
					el.options[sel2].style.cssText = el.options[sel].style.cssText;
					
					el.options[sel2].alreadymoved = true;
					el.options[sel2].selected = true;
					
					el.options[sel].value = strg;
					el.options[sel].text = strg1;
					el.options[sel].type = strg2;
					el.options[sel].style.cssText = st;
					el.options[sel].selected = false;
										
				}else //move group
				{
					for( var j = 0; j < el.childNodes.length; j++ ) 
					{
						if( ( el.childNodes[j] == grp ) && ( !el.childNodes[j-1] ) ) 
						{
							return res;
						}
						
						if( ( el.childNodes[j] == grp ) && ( el.childNodes[j-1] ) ) 
						{
							res = true;
							grp.swapNode( el.childNodes[j-1] );
							for( var k = 0; k < grp.childNodes.length; k++ )
							{
								grp.childNodes[k].alreadymoved = true;
							}
							break;
						}
					}	
					
				}
			}
			for( var i = 0; i < el.options.length; i++ ) 
			{
				el.options[i].alreadymoved = false;
			}
			
			setTimeout("set_scroll('"+s+"');", 1 );
			return res;
		}
		
	}
	return (0);
}

//-------------------------------------------------------------------
function set_scroll(name) 
{
	var el = document.getElementsByName( name );
	if( el ) el = el[0]; else return;
		
	for( var i = 0; i < el.options.length; i++ ) 
	//for( var i = el.options.length - 1; i > -1; i-- ) 
	{
		if ( el.options[i].selected ) 
		{
			el.options[i].selected = true;
		}
	}
}
function Move_Down(f, s) 
{
	var t=findInForm(f,s);
	if (t>-1) 
	{
		if( !f.elements[t].multiple )
		{
			var sel=f.elements[t].options.selectedIndex;
			if ((sel>-1)&&(sel<f.elements[t].length-1)) 
			{
				var strg=f.elements[t].options[sel+1].value;
				var strg1=f.elements[t].options[sel+1].text;
				var st = f.elements[t].options[sel+1].style.cssText;
	
				f.elements[t].options[sel+1].value=f.elements[t].options[sel].value;
				f.elements[t].options[sel+1].text=f.elements[t].options[sel].text;
				f.elements[t].options[sel+1].style.cssText = f.elements[t].options[sel].style.cssText;
	
				f.elements[t].options[sel].value=strg;
				f.elements[t].options[sel].text=strg1;
				f.elements[t].options[sel].style.cssText = st;
	
				f.elements[t].options.selectedIndex=sel+1;
				return (1);
			}
		}else
		{
			var el = f.elements[t];
			var all_grp = true;
			var res = false;
			
		
			for( var i = el.options.length - 1; i > -1; i-- )
			{
				if( ( !el.options[i].selected ) || ( el.options[i].alreadymoved ) ) { continue; } 
				
				if( el.options[i].parentElement.tagName != "OPTGROUP" )
				{//root
					for( var j = 0; j < el.childNodes.length; j++ ) 
					{
						if( ( el.childNodes[j] == el.options[i] ) && ( !el.childNodes[j+1] ) ) 
						{
							return res;
						}
						
						if( ( el.childNodes[j] == el.options[i] ) && ( el.childNodes[j+1] ) ) 
						{
							res = true;
							el.childNodes[j].alreadymoved = true;
							el.childNodes[j].swapNode( el.childNodes[j+1] );
							
							break;
						}
					}	
					continue;
				}
					
				var grp = el.options[i].parentElement;
				var grp_i;
				for( var j = 0; j < grp.childNodes.length; j++ )
				{
					if( !grp.childNodes[j].selected ) 
					{ 
						all_grp = false;
					}
					if( grp.childNodes[j] == el.options[i] ) 
					{
						grp_i = j;
					}
				}
				
				if( !all_grp ) //movement inside group
				{
										
					if( ( !grp.childNodes[ grp_i + 1 ] ) || ( grp.childNodes[ grp_i + 1 ].alreadymoved ) )
					{
						return res;
						//el.options[i].alreadymoved = true;
						//continue;
					}
					
					var sel = i;
					var sel2 = i + 1;
					res = true;
					var strg = el.options[sel2].value;
					var strg1 = el.options[sel2].text;
					var strg2 = el.options[sel2].type;
					var st = el.options[sel2].style.cssText;
		
					el.options[sel2].value = el.options[sel].value;
					el.options[sel2].text = el.options[sel].text;
					el.options[sel2].type = el.options[sel].type;
					el.options[sel2].style.cssText = el.options[sel].style.cssText;
					
					el.options[sel2].alreadymoved = true;
					el.options[sel2].selected = true;
					
					el.options[sel].value = strg;
					el.options[sel].text = strg1;
					el.options[sel].type = strg2;
					el.options[sel].style.cssText = st;
					el.options[sel].selected = false;
										
				}else //move group
				{
					for( var j = 0; j < el.childNodes.length; j++ ) 
					{
						if( ( el.childNodes[j] == grp ) && ( !el.childNodes[j+1] ) ) 
						{
							return res;
						}
						
						if( ( el.childNodes[j] == grp ) && ( el.childNodes[j+1] ) ) 
						{
							res = true;
							grp.swapNode( el.childNodes[j+1] );
							for( var k = 0; k < grp.childNodes.length; k++ )
							{
								grp.childNodes[k].alreadymoved = true;
							}
							break;
						}
					}	
					
				}
			}
			
			for( var i = 0; i < el.options.length; i++ ) 
			{
				el.options[i].alreadymoved = false;
			}
		
			setTimeout("set_scroll('"+s+"');", 1 );
			return res;
		}
	}
	return (0);
}

//-------------------------------------------------------------------
function delSelect(f, s) 
{
	var t=findInForm(f,s);
	var prov=0;
	if (t>-1) 
	{
		if( !f.elements[t].multiple )
		{
			var sel=f.elements[t].options.selectedIndex;
			if (sel>-1) 
			{
				f.elements[t].options[sel]=null;
				prov=1;
			}
			sel = (sel<f.elements[t].options.length) ? sel : sel-1;
			sel = (sel>-1) ? sel : 0;
			if (f.elements[t].options.length>0) 
				f.elements[t].options.selectedIndex = sel;
		}else
		{
			var i = 0;
			while( f.elements[t].options[i] )
			{
				if( f.elements[t].options[i].selected )
				{
					//f.elements[t].options[i] = null;
					f.elements[t].options[i].removeNode(true);
					prov = 1;
				}else i++;
			}
		}
	}
	return (prov);
}

//-------------------------------------------------------------------
function provSelect(f, s, value) 
{
	var t;
	t=findInForm(f, s);
	if (t>-1) 
	{
		var er=0;
		var i;
		for (i=0;i<f.elements[t].length;i++)
		{
			if (f.elements[t].options[i].value==value)
			{
				er=1;
				i=f.elements[t].length;
				return true;
			}
		}
	}
	return false;
}

//-------------------------------------------------------------------
function findInSelect(f, s, value) 
{
	var t;
	t=findInForm(f, s);
	if (t>-1) 
	{
		var er=0;
		var i;
		for (i=0;i<f.elements[t].length;i++)
		{
			if (f.elements[t].options[i].value==value)
			{
				er=1;
				return i;
			}
		}
	}
	return (-1);
}

//-------------------------------------------------------------------
function findPartInSelect(f, s, value) 
{
	var t;
	t=findInForm(f, s);
	if (t>-1) 
	{
		var er=0;
		var i;
		for (i=0;i<f.elements[t].length;i++)
		{
			if (f.elements[t].options[i].value.indexOf(value)>-1)
			{
				er=1;
				return i;
			}
		}
	}
	return (-1);
}

//-------------------------------------------------------------------
function convertSelectToString(f, s)
{
	var strg="";
	var t=findInForm(f,s);
	if (t>-1) 
	{
		var i=0;
		var l=f.elements[t].length;
		while (i<l) 
		{
			if (strg=="")
				strg=strg+f.elements[t].options[i].value;
			else
				strg=strg+", "+f.elements[t].options[i].value;
			i=i+1;
		}
	}
	return (strg);
}

//-------------------------------------------------------------------
function convertSelectNamesToString(f, s)
{
	var strg="";
	var t=findInForm(f,s);
	if (t>-1) 
	{
		var i=0;
		var l=f.elements[t].length;
		while (i<l) 
		{
			if (strg=="")
				strg=strg+f.elements[t].options[i].text;
			else
				strg=strg+", "+f.elements[t].options[i].text;
			i=i+1;
		}
	}
	return (strg);
}

//-------------------------------------------------------------------

function ReplaceToAmp( sStrg )
{
	//var re = /&amp;/ig;
	return replace(ReplaceAmp(sStrg), "&", "&amp;");
}


//-------------------------------------------------------------------
function ReplaceAmp( sStrg )
{
	var re = /&amp;/ig;
	return( sStrg.replace( re, "&" ) );
}

//-------------------------------------------------------------------
function QuotWithAmp( txtval )
{
	var res=replace(txtval, "&quot;", "\"");
	res=replace(res, "&lt;", "<");
	res=replace(res, "&gt;", ">");
	res=replace(res, "&#39;", "'");
	res=replace(res, "&amp;", "&");
	return (res);
}
//-------------------------------------------------------------------
function Quot( txtval )
{
	var res=replace(txtval, "&quot;", "\"");
	res=replace(res, "&lt;", "<");
	res=replace(res, "&gt;", ">");
	res=replace(res, "&#39;", "'");
	return (res);
}
//-------------------------------------------------------------------
function NoQuot(txtval) 
{
	var res=replace(txtval, "\"", "&quot;");
	res=replace(res, "<", "&lt;");
	res=replace(res, ">", "&gt;");
	res=replace(res, "'", "&#39;");
	return (res);
}

//-------------------------------------------------------------------
function NoQuotWithAmp(txtval) 
{
	var res=ReplaceToAmp(txtval);
	res=replace(res, "\"", "&quot;");
	res=replace(res, "<", "&lt;");
	res=replace(res, ">", "&gt;");
	res=replace(res, "'", "&#39;");
	return (res);
}

//-------------------------------------------------------------------
function NoQuotForScript(txtval) 
{
	var res=replace(txtval, "\"", "\\\"");
	return (res);
}

//-------------------------------------------------------------------
function replaceComma(txtval)
{
	var r=/,/g;
	return(txtval.replace(r, "%2C"));
}

//-------------------------------------------------------------------
function Bluring(f,el) 
{
	var t=findInForm(f,el);
	if (t>-1) 
	{
		f.elements[t].blur();
	}
}

//-------------------------------------------------------------------
function findInArray(arr,value) 
{
	var i;
	for (i=0;i<arr.length;i++) 
		if (arr[i]==value)
			return(i);
	return (-1);
}

//-------------------------------------------------------------------
function addValueToHidden(form, element, value) 
{
	var val_old = getField(form, element);
	setField(form, element, ( val_old != "" ? val_old + ", " : "" ) + value);
}

//-------------------------------------------------------------------
function replace(s,s1,s2)
{
	var res="";
	if (s!=null) 
	{
		var n=s.indexOf(s1,0);
		var n1=0;
		if (n>=0) 
		{
			while (n>=0) 
			{
				res=res+s.substring(n1,n)+s2;
				n1=n+s1.length;
				var k=s1.length;
				n=s.indexOf(s1,n+k);
			}
			var ty=s.length;
			res=res+s.substring(n1,ty);
		}
		else
		{
			res=s;
		}
	}
	return (res);
}

//-------------------------------------------------------------------
function findInForm(f,n) 
{
	var i;
	for (i=0;i<f.length;i++)
		if (f.elements[i].name.toLowerCase()==n.toLowerCase())
			return (i);
	return (-1);
}

//-------------------------------------------------------------------
function getField(f,n) 
{
	var z;
	z=findInForm(f,n);
	if (z>=0) 
	{
		return (f.elements[z].value);
	}
	else
		return ("");
}

//-------------------------------------------------------------------
function getChecked(f,n) 
{
	var z;
	z=findInForm(f,n);
	if (z>=0)
		return (f.elements[z].checked);
	else
		return ("false");
}

//-------------------------------------------------------------------
function setRadio(f, n, value)
{
	var i;
	var els = f.elements(n);
	for (i=0;i<els.length;i++)
	{
		if ((els[i].name.toLowerCase()==n.toLowerCase()) && (els[i].value+""==value+""))
		{
			els[i].checked=true;
			return;
		}
	}
}

//-------------------------------------------------------------------
function getRadio(f,n) 
{
	var i;
	var els = f.elements(n);
	for (i=0;i<els.length;i++)
	{
		if ((els[i].name.toLowerCase()==n.toLowerCase()) && (els[i].checked==true))
		{
			return (els[i].value);
		}
	}
	return ("");
}

//-------------------------------------------------------------------
function setSelect(f,n,v)
{
	var z;
	z = f.elements(n);
	if (typeof(z)!="undefined")
	{
		for (i=0;i<z.length;i++) 
		{
			if (z[i].value+""==v+"")
			{
				z.selectedIndex=i;
				return( true );
			}
		}
	}
	return( false );
}

//-------------------------------------------------------------------
function getSelect(f,n) 
{
	var z;
	z = f.elements(n);
	if (typeof(z)!="undefined")
	{
		if (z.selectedIndex>-1)
			return (z[z.selectedIndex].value);
		else
			return ("");
	}
	else
		return ("");
}

//-------------------------------------------------------------------
function getNumberOfActiveRadio(f,n) 
{
	var i;
	var k=0;
	for (i=0;i<f.length;i++)
		if ((f.elements[i].name.toLowerCase()==n.toLowerCase()))
		 if (f.elements[i].checked==true)
			return (k)
		else
			k=k+1;
	return (-1);
}

//-------------------------------------------------------------------
function setField(f,n,v) 
{
	var z;
	z=findInForm(f,n);
	if (z>-1)
		f.elements[z].value=v;
}

//-------------------------------------------------------------------
function setChecked(f,n,v) 
{
	var z;
	z=findInForm(f,n);
	if (z>-1)
		f.elements[z].checked=v;
}

//-------------------------------------------------------------------
function check()
{
	if (check_int()) 
	{
		var er=0;
		var j=1;
		i=0;
		while (j!=0) 
		{
			if ((document.forms(0).elements[i].name.substring(0,5)=="need_") && (document.forms(0).elements[i].value=="1")) 
			{
				s="fill_"+document.forms(0).elements[i].name.substring(5,document.forms(0).elements[i].name.length);
				s1=document.forms(0).elements[i].name.substring(5,document.forms(0).elements[i].name.length);
				if ((getField(document.forms(0),s)=="")||(getField(document.forms(0),s)=="false"))
				{
					er=1;
					alert("You didn't fill the field '"+getField(document.forms(0),"name_"+s1)+"'");
					break;
				}
			}
			else
				j=0;
			i=i+1;
		}
		if (er==0)
			document.forms(0).submit();
		else
			return (false)
	}
	else 
	{
		return false
	}
}

//-------------------------------------------------------------------
function DisplayErrorMessage( err )
{
	if( typeof( err ) == "object" )
	{
		alert( err.description );
	}
	else
	{
		alert( err );
	}
}
