using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Propolis
{
    public enum PropolisRecipeCompareStatus {DIFFERENT,IMPERFECT,PERFECT}
    public class PropolisRecipe
    {
        public int[] _recipe;
        public PropolisRecipe()
        {
            _recipe = new int[3];
        }
        public PropolisRecipe(int recipeItem1, int recipeItem2, int recipeItem3)
        {
            _recipe = new int[3];
            _recipe[0] = recipeItem1;
            _recipe[1] = recipeItem2;
            _recipe[2] = recipeItem3;
        }

        public PropolisRecipe(PropolisStatus recipeItem1, PropolisStatus recipeItem2, PropolisStatus recipeItem3)
        {
            _recipe = new int[3];
            _recipe[0] = (int)recipeItem1;
            _recipe[1] = (int)recipeItem2;
            _recipe[2] = (int)recipeItem3;
        }



        public void SetItem(PropolisStatus status, int index)
        {
            try
            {
                _recipe[index] = (int)status;
            }
            catch (Exception)
            {

               
            }
        }


        public void SetItem(int status, int index)
        {
            try
            {
                _recipe[index] = status;
            }
            catch (Exception)
            {


            }
        }

        public int GetItem(int index)
        {
            try
            {
                return _recipe[index];
            }
            catch (Exception)
            {
                return -1;

            }
        }

        public static PropolisRecipe ParseRecipe(AbstractGroup groupData)
        {
            PropolisRecipe recipe = new PropolisRecipe();
            if(groupData.ChildItemsList.Count != 10 && groupData.ChildItemsList.Count != 4)
            {
                return null;
            }

            List<PropolisStatus> items = groupData.ChildItemsList.Select(x=>x.Status).ToList();

            for (int i = 0; i < 3; i++)
            {
                if (items[i*3] == items[i * 3 + 1] && items[i * 3] == items[i * 3 + 2])
                {
                    recipe.SetItem(items[i* 3], i);
                }
                else {
                    return null;
                }
            }

            return recipe;
        }

        public PropolisRecipeCompareStatus CompareTo(PropolisRecipe recipe)
        {
            if(recipe == null )
            {
                return PropolisRecipeCompareStatus.DIFFERENT;
            }


            if(this.GetItem(0) == recipe.GetItem(0) &&
                this.GetItem(1) == recipe.GetItem(1)&&
                this.GetItem(2) == recipe.GetItem(2))
            {
                return PropolisRecipeCompareStatus.PERFECT;
            }
            else
            {
                List<int> recipeStack = new List<int>(_recipe);

                for(int i = 0; i < 3; i++)
                {
                    try
                    {
                        if (recipeStack.IndexOf(recipe.GetItem(i)) != -1)
                        {
                            recipeStack.Remove(recipe.GetItem(i));
                        }
                        else
                        {

                            return PropolisRecipeCompareStatus.DIFFERENT;
                        }
                        if(recipeStack.Count == 0)
                        {
                            return PropolisRecipeCompareStatus.IMPERFECT;
                        }
                    }
                    catch (Exception)
                    {

                        return PropolisRecipeCompareStatus.DIFFERENT;
                    }
                  
                }
            }



            return PropolisRecipeCompareStatus.DIFFERENT;

        }
    }
}
