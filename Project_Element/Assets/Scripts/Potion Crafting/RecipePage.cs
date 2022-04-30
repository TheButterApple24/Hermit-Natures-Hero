using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipePage : MonoBehaviour
{
    //public CraftingMenu m_CraftingMenu;
    //public Text m_RecipeList;
    
    // Start is called before the first frame update
    void Start()
    {
        //foreach (CraftingRecipe recipe in m_CraftingMenu.m_CraftingRecipes)
        //{
        //    m_RecipeList.text += recipe.Reagent1.ToString() + " + " + recipe.Reagent2.ToString() + " = " + recipe.ResultingPotion.ToString() + "\n";
        //}
    }
    public void OpenRecipeList()
    {
        gameObject.SetActive(true);
        //m_CraftingMenu.gameObject.SetActive(false);
    }

    public void CloseRecipeList()
    {
        gameObject.SetActive(false);
        //m_CraftingMenu.gameObject.SetActive(true);
    }
}
