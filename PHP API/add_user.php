<?php

    //Prevents access without API key
    $key = $_POST['server_api_key'];
    if($key != "")
    {
        die();
    }

    require 'connect.php';
    require 'validations.php';

    $user_id = $_POST['user_id'];
    if(CheckIDExists($user_id, $conn) == false && $user_id != 0)
    {
         //Prepare SQL statement beforehand to prevent SQL Injection
        $stmt = $conn->prepare("INSERT INTO tbl_users VALUES(?,?,?,?,?,?)");
        $stmt->bind_param("iiiiis", $user_id,$room_id,$score,$tasksRevived,$tasksCompleted,$username);
    
        //Prepare values
        $room_id = 0;
        $score = 0;
        $tasksRevived = 0;
        $tasksCompleted = 0;
        $username = "anon_" .$user_id;

        //Execute prepared statement
        try
        {
            $stmt->execute();
            echo $user_id;
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
        echo 'ID ALREADY EXISTS';
    }
   
?>