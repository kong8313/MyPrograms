function appletLocalJournal(regkey) {
try {
	document.write ('<applet code="physicon.journal.LocalJournal.class" name="LocalJournal" width="1" height="1" background="008080" foreground="FFFFFF">');
	document.write ('<param name="jfname_regkey" value="'+regkey+'"/>');
	document.write ('</applet>');
}catch(e){}
}

function objectputflash (width, height, path) {
try {
	document.write ('<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=5,0,0,0" width="' + width + '" height="' + height + '">');
	document.write ('<param name="movie" value="'+path+'"/>');						
	document.write ('<param name="quality" value="high"/>');
	document.write ('<param name="wmode" value="transparent"/>');
	document.write ('<param name="devicefont" value="true"/>');
	document.write ('<param name="bgcolor" value="#FFFFFF"/>');							
	document.write ('</object>');
	//document.write ('<a href="'+path+'" disabled="true"/>');
}catch(e){}	
}
function objectputtestbar (width, height, path) {
try {
	document.write ('<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=5,0,0,0" id="testbar" width="' + width + '" height="' + height + '">');
	document.write ('<param name="movie" value="'+path+'"/>');
	document.write ('<param name="quality" value="high"/>');
	document.write ('<param name="wmode" value="transparent"/>');
	document.write ('<param name="bgcolor" value="#FFFFFF"/>');
	document.write ('<param name="menu" value="false"/>');
	document.write ('</object>');
}catch(e){}
}
