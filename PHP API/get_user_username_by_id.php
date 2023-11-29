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

    if(CheckIDExists($user_id,$conn) == true)
    {
        //Prepare SQL statement beforehand to prevent SQL Injection
        $stmt = $conn->prepare("SELECT username FROM tbl_users WHERE user_id = ?");
        $stmt->bind_param("i",$user_id);
    
        //Execute prepared statement
        try
        {
            $stmt->execute();
            $stmt->bind_result($result); 
            $stmt->fetch();
        
            echo $result;
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
        echo 'ID NOT FOUND';
    }

?>