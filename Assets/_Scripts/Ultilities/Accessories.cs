/*Alex Greff
19/01/2016
Accessories
My static utilites class
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DirectionRoundTypes {
    ALL, VERTICAL, HORIZONTAL
}

//Static class
public static class Accessories {
    private const string KEY = "117TMHR@Con"; //The key

    //FTP login information
    private const string ftp_ADDRESS = @"ftp://mrcomputers.info/";
    private const string ftp_USERNAME = "GreffAlexander@mrcomputers.info";
    private const string ftp_PASSWORD = "0001787478";

    public static readonly string WEB_DATA_ROOT_PATH = "summative/_data/"; //The mrcomputers web path

    public static string LOCAL_DATA_ROOT_PATH { //The local path on the machine
        get {
            return Application.persistentDataPath.ToString() + "/_data/";
        }
    }

    public static readonly string REGISTERED_USERS_FILE_PATH = "registeredUsers.txt";
    public static readonly string CURRENT_CREDENTIALS_FILE_PATH = "currentCredentials.txt";

    public static readonly string REGISTER_URL_LINK = "http://mrcomputers.info/mrcomputers.info/GreffAlexander/summative/login.php";

    //Login credential methods
    public static string FTP_ADDRESS (string key) {
        if (key.Equals(KEY))
            return ftp_ADDRESS;
        else
            return null;
    }

    public static string FTP_USERNAME (string key) {
        if (key.Equals(KEY))
            return ftp_USERNAME;
        else
            return null;
    }

    public static string FTP_PASSWORD (string key) {
        if (key.Equals(KEY))
            return ftp_PASSWORD;
        else
            return null;
    }

    //EVIDENCE: Static variables
    private static Vector2[] allVectorDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right }; //Possible directions
    private static Vector2[] verticalVectors = { Vector2.up, Vector2.down };
    private static Vector2[] horizontalVectors = { Vector2.left, Vector2.right };

    //EVIDENCE: Static method
    //Returns the closest whole vector2 direction (up, down, right, left) of a vector2 parameter
    public static Vector2 ClosestDirection (Vector2 v, DirectionRoundTypes type) {
        if (type == DirectionRoundTypes.VERTICAL) { //Round to up or down
            float maxDot = -Mathf.Infinity;
            Vector2 ret = Vector2.zero;

            foreach (Vector2 dir in verticalVectors) {
                float dot = Vector2.Dot(v, dir);
                if (dot > maxDot) {
                    ret = dir;
                    maxDot = dot;
                }
            }
            return ret;
        }
        else if (type == DirectionRoundTypes.HORIZONTAL) { //Round to right or left
            float maxDot = -Mathf.Infinity;
            Vector2 ret = Vector2.zero;

            foreach (Vector2 dir in horizontalVectors) {
                float dot = Vector2.Dot(v, dir);
                if (dot > maxDot) {
                    ret = dir;
                    maxDot = dot;
                }
            }
            return ret;
        }
        else { //Round to all directions
            float maxDot = -Mathf.Infinity;
            Vector2 ret = Vector2.zero;

            foreach (Vector2 dir in allVectorDirections) {
                float dot = Vector2.Dot(v, dir);
                if (dot > maxDot) {
                    ret = dir;
                    maxDot = dot;
                }
            }
            return ret;
        }
    }

    //EVIDENCE: insertion sorting
    public static int[] SortScores (int[] arr) {
        for (int i = 1; i < arr.Length; i++) { //Iterate from 1 to the array length -1 
			int key = arr[i]; //Set the key 
			int j = i; //Set j to i
			
			while (j > 0 && arr[j-1] > key) { //While j is above zero (makes sure it doesn't go out of bounds) and the element one below the key is less than the key
				arr [j] = arr[j - 1]; //First part of the swap (insert the higher value one up)
				j--; //Decrement
			} 
			
			arr[j] = key; //Finish the swap (insert the lower value down) 
		}

        return arr;
    }

    //EVIDENCE: binary search
    //String binary search
    public static int searchFor (string searchString, string[] arr){ //Returns the index that it's at
		int f = 0; //First index
		int l = arr.Length - 1; //Last index
		int m = (f+l)/2; //Middle index
		
		while (f <= l) { //While the first index is less than or equal to the last index (once they cross we know that the String we were looking for was not found)
			if (arr[m].CompareTo(searchString) < 0) { //If the middle string is alphabetically to the right of the search word
				f = m + 1; //Set the first index to the middle point plus one
			}
			else if (arr[m].Equals(searchString)) { //If we found the term we're looking for
                return m; //Return it
			}
			else { //If the middle string is alphabetically to the left of the search word
				l = m - 1; //Set the last index to the middle point plus one
			}
			
			m = (f+l)/2; //Recalculate the middle point
		}
        return -1; //If it gets here then it means the search term wasn't found so just return it as -1
	}
	
    //Integer binary search
	public static int searchFor (int searchInt, int[] arr){ //Returns the index that it's at
		int f = 0;
		int l = arr.Length - 1;
		
		while (f <= l) {
			int m = (f+l)/2;
			
			if (m < searchInt) {
				f = m + 1;
			}
			else if (arr[m] == searchInt) {
				return m;
			}
			
			else {
				l = m - 1;
			}
		}
		return -1;
	}

    //EVIDENCE: recursion
    public static string reverse (string word) { 
		if (word.Length == 1) //Base case
			return word;
		else 
            //Add the last letter of the word to the remaining letters of the word
			return word.ToCharArray()[word.Length -1] + reverse (word.Substring(0, word.Length-1)); 
	}

    public static Vector2 RadianToVector2(float radian) {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }
  
    public static Vector2 DegreeToVector2(float degree) {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static float time { //Returns current time
        get {
            if (PauseManager.Instance != null)
                return Time.time - PauseManager.Instance.TimeSetBack;
            else
                return Time.time;
        }
    }
}
