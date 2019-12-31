<?php
	session_start();
	session_regenerate_id();	//Create a copy of the current session
	session_unset();			//Wipe all the variables stored from the previous session
	
	//Save sample variables into the new (empty) session
	$_SESSION['number'] = 12;
	$_SESSION['float'] = 67.14;
	$_SESSION['double'] = 15.81975548315756;
	$_SESSION['yes'] = true;
	$_SESSION['no'] = false;
	$_SESSION['text'] = "abc defg hij";
	$_SESSION['array_words'] = ['first','second','third'];
	$_SESSION['array_nums'] = [1000,2000,3000];
	$_SESSION['array_assoc'] = array('male' => 'man', 'female' => 'woman');
	$_SESSION['array_multi'] = [[1.1,1.2,1.3],[2.1,2.2,2.3],[3.1,3.2,3.3]];
	
	//Show id of the new session
	echo "<pre>Test completed succesfully.</pre>";
	echo "<pre>Session ID: ".session_id()."</pre>";
?>