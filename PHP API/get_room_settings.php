<?php

    //Prevents access without API key
    $key = $_POST['server_api_key'];
    if($key != "")
    {
        die();
    }

    require 'connect.php';
    require 'validations.php';
    
    //Prepare values
    $room_id = $_POST['room_id'];

    if(CheckRoomExists($room_id,$conn) == true)
    {
        //Prepare SQL statement beforehand to prevent SQL Injection
        $stmt = $conn->prepare("SELECT * FROM tbl_rooms WHERE room_id = ?");
        $stmt->bind_param("i",$room_id);
    
        //Execute prepared statement
        try
        {
            $rows = array();
            $stmt->execute();
            $result = $stmt->get_result();
            $row = $result->fetch_assoc();
            
            echo implode(",", $row);
        }
        catch(Exception $e)
        {
            echo $e->getMessage();
        }

        $stmt->close();
        $conn->close();
    }
    else
    {
        echo 'ROOM NOT FOUND';
    }

?>