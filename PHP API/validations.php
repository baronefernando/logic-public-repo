<?php
    function CheckIDExists($user_id,$conn)
    {
        //Prepare SQL statement beforehand to prevent SQL Injection
        $stmt = $conn->prepare("SELECT COUNT(1) FROM tbl_users WHERE user_id = ?");
        $stmt->bind_param("i", $user_id);
    
        //Execute prepared statement
        try
        {
            $stmt->execute();
            $stmt->bind_result($count);
            $stmt->fetch();

            if($count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception $e)
        {
            echo $e->getMessage();
        }

        $stmt->close();
        $conn->close();
    }

    function CheckRoomExists($room_id,$conn)
    {
        //Prepare SQL statement beforehand to prevent SQL Injection
        $stmt = $conn->prepare("SELECT COUNT(1) FROM tbl_rooms WHERE room_id = ?");
        $stmt->bind_param("i", $room_id);

        //Execute prepared statement
        try
        {
            $stmt->execute();
            $stmt->bind_result($count);
            $stmt->fetch();

            if($count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception $e)
        {
            echo $e->getMessage();
        }

        $stmt->close();
        $conn->close();
    }

    function CheckUsername($username,$user_id,$conn)
    {
        //Prepare SQL statement beforehand to prevent SQL Injection
        $stmt = $conn->prepare("SELECT COUNT(1) FROM tbl_users WHERE username = ? AND user_id != ?");
        $stmt->bind_param("si", $username,$user_id);

        //Execute prepared statement
        try
        {
            $stmt->execute();
            $stmt->bind_result($count);
            $stmt->fetch();

            if($count > 0)
            {
                echo "USERNAME TAKEN";
                die();
            }
            else
            {
                return true;
            }
        }
        catch(Exception $e)
        {
            echo $e->getMessage();
        }

        $stmt->close();
        $conn->close();
    }


    function TryToOpenRoom($room_id,$room_pin,$conn)
    {
        if($room_id == 0)
        {
            return true;
        }

        if(CheckRoomExists($room_id,$conn) == true)
        {
            //Prepare SQL statement beforehand to prevent SQL Injection
            $stmt = $conn->prepare("SELECT room_locked FROM tbl_rooms WHERE room_id = ?");
            $stmt->bind_param("i", $room_id);

            //Execute prepared statement
            try
            {
                $stmt->execute();
                $stmt->bind_result($result); 
                $stmt->fetch();
                $stmt->close();

                if($result == 1 && $room_pin == 0)
                {
                    echo "Locked";
                    die();
                }
                elseif ($result == 1 && $room_pin > 0) 
                {
                    //Prepare SQL statement beforehand to prevent SQL Injection
                    $stmt = $conn->prepare("SELECT COUNT(1) FROM tbl_rooms WHERE room_id = ? AND room_pin = ?");
                    $stmt->bind_param("ii", $room_id,$room_pin);

                    $stmt->execute();
                    $stmt->bind_result($pinresult); 
                    $stmt->fetch();
                    $stmt->close();

                    if($pinresult == 0)
                    {
                        echo "Wrong";
                        die();
                    }
                    else
                    {
                        return true;
                    }

                }
                else if($result == 0)
                {
                    return true;
                }
            }
            catch(Exception $e)
            {
                echo $e->getMessage();
            }
            $conn->close();
        }
        else
        {
            echo "ROOM DOES NOT EXIST";
            die();
        }
    }

    function RemoveEmptyRooms($target_room_occupation,$conn)
    {
        //Prepare SQL statement beforehand to prevent SQL Injection
        $stmt = $conn->prepare("DELETE FROM tbl_rooms WHERE room_occupation = ?");
        $stmt->bind_param("i", $target_room_occupation);

        //Execute prepared statement
        try
        {
            $stmt->execute();
            return;
        }
        catch(Exception $e)
        {
            echo $e->getMessage();
        }

        $stmt->close();
        $conn->close();
    }
?>