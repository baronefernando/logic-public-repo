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
        $stmt = $conn->prepare("SELECT * FROM tbl_users WHERE room_id = ?");
        $stmt->bind_param("i", $room_id);
    
        //Execute prepared statement
        try
        {
            $stmt->execute();
            $result = $stmt->get_result();
            $rows = array();
            while($row = $result->fetch_assoc())
            {
                $rows[] = $row;
            }
            echo json_encode($rows);
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
        echo 'ROOM DOES NOT EXIST';
    }
?>