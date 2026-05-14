function moveElements( from_id, hfrom_id, to_id, hto_id )
{
	var group_from = document.getElementById( from_id );
	var group_to = document.getElementById( to_id );
	var h_from = document.getElementById( hfrom_id );
	var h_to = document.getElementById( hto_id );
	var i = group_from.selectedIndex;
	if( i > -1 )
	{
		var item = group_from.options[i];
		group_from.options.remove( i );
		group_to.options.add( item );

		if( group_from.options.length <= i )
			group_from.selectedIndex = i - 1;
		else
			group_from.selectedIndex = i;
	}
	
	h_from.value = '';
	for( var i=0; i<group_from.options.length; i++ )
		h_from.value = h_from.value + ';' + group_from.options[i].value;
		
	h_to.value = '';
	for( var i=0; i<group_to.options.length; i++ )
	    h_to.value = h_to.value + ';' + group_to.options[i].value;
    
    if (window.StateChecker) {
        window.StateChecker.MarkAsChanged();
    }
}

function SetEnabledDoubleList(control_id, b_state)
{
	var elem = document.getElementById( control_id + "_rightList" );
	elem.disabled = !b_state;
	elem = document.getElementById( control_id + "_leftList" );
	elem.disabled = !b_state;
	elem = document.getElementById( control_id + "_bttnRight" );
	elem.disabled = !b_state;
	elem = document.getElementById( control_id + "_bttnLeft" );
	elem.disabled = !b_state;
	
}
