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
    $columns_to_update = $_POST['columns_to_update'];

    $owner_id = $_POST['owner_id'];
    $room_locked = $_POST['room_locked'];
    $room_pin = $_POST['room_pin'];
    $room_capacity = $_POST['room_capacity'];
    $room_occupation = $_POST['room_occupation'];
    $game_started = $_POST['game_started'];

    if(CheckRoomExists($room_id,$conn) == true)
    {
        //Prepare SQL statement beforehand to prevent SQL Injection
        if($columns_to_update == 1)
        {
            if(CheckIDExists($owner_id,$conn))
            {
                $stmt = $conn->prepare("UPDATE tbl_rooms SET owner_id = ? WHERE room_id = ?");
                $stmt->bind_param("ii",$owner_id,$room_id);
            }
            else
            {
                echo 'USER NOT FOUND';
                die();
            }
        }
        else if ($columns_to_update == 2)
        {
            $stmt = $conn->prepare("UPDATE tbl_rooms SET room_locked = ? WHERE room_id = ?");
            $stmt->bind_param("ii",$room_locked,$room_id);
        }
        else if ($columns_to_update == 3)
        {
            $stmt = $conn->prepare("UPDATE tbl_rooms SET room_pin = ? WHERE room_id = ?");
            $stmt->bind_param("ii",$room_pin,$room_id);
        }
        else if ($columns_to_update == 4)
        {
            $stmt = $conn->prepare("UPDATE tbl_rooms SET room_capacity = ? WHERE room_id = ?");
            $stmt->bind_param("ii",$room_capacity,$room_id);
        }
        else if ($columns_to_update == 5)
        {
            $stmt = $conn->prepare("UPDATE tbl_rooms SET room_occupation = ? WHERE room_id = ?");
            $stmt->bind_param("ii",$room_occupation,$room_id);
        }
        else if ($columns_to_update == 6)
        {
            $stmt = $conn->prepare("UPDATE tbl_rooms SET room_locked = ?, room_pin = ?, room_capacity = ?, game_started = ? WHERE room_id = ?");
            $stmt->bind_param("iiiii",$room_locked,$room_pin,$room_capacity,$game_started,$room_id);    
        }
        else if ($columns_to_update == 7)
        {
            $stmt = $conn->prepare("UPDATE tbl_rooms SET game_started = ? WHERE room_id = ?");
            $stmt->bind_param("ii",$game_started,$room_id);    
        }
    
        //Execute prepared statement
        try
        {
            $stmt->execute();
            echo 'Updated';
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