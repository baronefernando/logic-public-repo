<?php

    //Prevents access without API key
    $key = $_POST['server_api_key'];
    if($key != "")
    {
        die();
    }

    require 'connect.php';
    require 'validations.php';

    $room_id = $_POST['room_id'];
    RemoveEmptyRooms(0,$conn);

    if(CheckRoomExists($room_id,$conn) == false)
    {
        //Prepare SQL statement beforehand to prevent SQL Injection
        $stmt = $conn->prepare("INSERT INTO tbl_rooms VALUES(?,?,?,?,?,?,?)");
        $stmt->bind_param("iiiiiii", $room_id,$owner_id,$room_locked,$room_pin,$room_capacity,$room_occupation,$game_started);
    
        //Prepare values
        $owner_id = $_POST['owner_id'];
        $room_locked = $_POST['room_locked'];
        $room_pin = $_POST['room_pin'];
        $room_capacity = $_POST['room_capacity'];
        $room_occupation = 0;
        $game_started = 0;

        //Execute prepared statement
        try
        {
            $stmt->execute();
            echo $room_id;
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
        echo 'ROOM ALREADY EXISTS';
    }
?>