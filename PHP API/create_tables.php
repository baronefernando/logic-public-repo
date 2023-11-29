<?php
	require 'connect.php';

	$query_tbl_users = "CREATE TABLE tbl_users 
						(
							user_id INT NOT NULL, 
							room_id INT NOT NULL,
							score INT NOT NULL,
							tasksRevived INT NOT NULL,
							tasksCompleted INT NOT NULL,
							username VARCHAR(30),
							PRIMARY KEY (user_id)
						)";
				
	$query_tbl_rooms = "CREATE TABLE tbl_rooms 
						(
							room_id  INT NOT NULL,
							owner_id INT NOT NULL,
							room_locked BOOLEAN NOT NULL,
							room_pin INT,
							room_capacity INT,
							room_occupation INT,
							game_started BOOLEAN,
							PRIMARY KEY (room_id), 
							FOREIGN KEY (owner_id) REFERENCES tbl_users(user_id)
						)";

	$result_tbl_users = $conn->query($query_tbl_users);
	$result_tbl_rooms = $conn->query($query_tbl_rooms);

	if(!$result_tbl_users || !$result_tbl_rooms)
	{
		echo "Error creating tables.";
		echo $conn->error;
	}

	$conn->close();
?>
