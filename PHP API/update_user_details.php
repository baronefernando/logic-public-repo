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
    $user_id = $_POST['user_id'];
    $columns_to_update = $_POST['columns_to_update'];
    
    $room_id = $_POST['room_id'];
    $old_room_id = $_POST['old_room_id'];
    $score = $_POST['score'];
    $tasksRevived = $_POST['tasksRevived'];
    $tasksCompleted = $_POST['tasksCompleted'];
    $username = $_POST['username'];
    $room_pin = $_POST['room_pin'];

    if(CheckIDExists($user_id,$conn) == true)
    {
        //Prepare SQL statement beforehand to prevent SQL Injection
        if($columns_to_update == 1)
        {
            TryToOpenRoom($room_id,$room_pin,$conn);
            $stmt = $conn->prepare("UPDATE tbl_users SET room_id = ? WHERE user_id = ?");
            $stmt->bind_param("ii",$room_id,$user_id);
        }
        else if ($columns_to_update == 2)
        {
            $stmt = $conn->prepare("UPDATE tbl_users SET score = ? WHERE user_id = ?");
            $stmt->bind_param("ii",$score,$user_id);
        }
        else if ($columns_to_update == 3)
        {
            CheckUsername($username,$user_id,$conn);
            $stmt = $conn->prepare("UPDATE tbl_users SET username = ? WHERE user_id = ?");
            $stmt->bind_param("si",$username,$user_id);
        }
        else if ($columns_to_update == 6)
        {
            $stmt = $conn->prepare("UPDATE tbl_users SET room_id = ?, score = ?, username = ? WHERE user_id = ?");
            $stmt->bind_param("iiiiii",$room_id,$score,$username,$user_id);    
        }
        else if ($columns_to_update == 7)
        {
            $stmt = $conn->prepare("UPDATE tbl_users SET score = ?, tasksRevived = ?, tasksCompleted = ? WHERE user_id = ?");
            $stmt->bind_param("iiii",$score,$tasksRevived,$tasksCompleted,$user_id);
        }
        else if ($columns_to_update == 8)
        {
            $stmt = $conn->prepare("UPDATE tbl_users SET score = ?, tasksRevived = ?, tasksCompleted = ? WHERE room_id = ?");
            $stmt->bind_param("iiii",$score,$tasksRevived,$tasksCompleted,$room_id);
        }

    
        //Execute prepared statement
        try
        {
            $stmt->execute();
            echo 'Updated';
            
            if($columns_to_update == 1)
            {
                if($room_id == 0)
                {
                    $room_id = $old_room_id;
                }
            
                $stmt->close();
                $stmt = $conn->prepare("SELECT COUNT(1) FROM tbl_users WHERE room_id = ?");
                $stmt->bind_param("i",$room_id);
                $stmt->execute();
                $stmt->bind_result($occ); 
                $stmt->fetch();
                $stmt->close();

                $stmt = $conn->prepare("UPDATE tbl_rooms SET room_occupation = ? WHERE room_id = ?");
                $stmt->bind_param("ii",$occ,$room_id);
                $stmt->execute();
                RemoveEmptyRooms(0,$conn);
            }
            
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
        echo 'ID DOES NOT EXIST';
    }
?>