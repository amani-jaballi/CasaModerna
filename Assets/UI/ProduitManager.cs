using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class ProduitManager : MonoBehaviour
{
    FirebaseFirestore db;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                LoadProduits();
            }
            else
            {
                Debug.LogError("Firebase not ready");
            }
        });
    }

    void LoadProduits()
    {
        db.Collection("produits").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Dictionary<string, object> data = document.ToDictionary();

                    string id = document.Id;
                    string name = data["name"].ToString();
                    string color = data["color"].ToString();
                    string material = data["material"].ToString();
                    string price = data["price"].ToString(); //  string

                    Debug.Log("------");
                    Debug.Log("ID: " + id);
                    Debug.Log("Name: " + name);
                    Debug.Log("Price: " + price);
                    Debug.Log("Color: " + color);
                    Debug.Log("Material: " + material);
                }
            }
        });
    }
}