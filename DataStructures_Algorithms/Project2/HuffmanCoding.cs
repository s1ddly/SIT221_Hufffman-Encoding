using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures_Algorithms.Project1;

namespace DataStructures_Algorithms.Project2
{
    // Material
    // https://en.wikipedia.org/wiki/Huffman_coding
    // https://www.siggraph.org/education/materials/HyperGraph/video/mpeg/mpegfaq/huffman_tutorial.html
    // http://www.geeksforgeeks.org/greedy-algorithms-set-3-huffman-coding/

    public class node // node class for huffman tree
    {
        //list of variables for the node class
        public node father { get; set; }
        public node right { get; set; }
        public node left { get; set; }
        public int weight { get; set; }
        public string value { get; set; }
        public bool na { get; set; }
        public string code { get; set; }

        public node(int w, string v, node r = null, node l = null, node f = null)//initialiser constructor for new object
        {
            father = f;
            right = r;
            left = l;
            weight = w;
            value = v;
        }

        public node(node n)//copy constructor
        {
            father = n.father;
            right = n.right;
            left = n.left;
            weight = n.weight;
            value = n.value;
            na = n.na;
            code = n.code;
        }
    }

    public class tree
    {
        public List<node> nodelist{ get; set; }//this is the tree, which in thic case is just a list
    }

    public class HuffmanCoding
    {
        public List<char> inlist = new List<char>();//list of input variables
        public tree huffmantree = new tree();//declare tree

        public bool membership(char x)//membership method to check if item is member of inlist
        {
            if (inlist is null)
            {
                return false;
            } else
            {
                foreach (char i in inlist)
                {
                    if (i == x)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Vector<string> Encode(Vector<char> input)//beginning of encoding
        {
            foreach(char x in input)//populate inlist
            {
                if(membership(x) == false)//check membership in inlist, if false, put in inlist
                {
                    inlist.Add(x);
                }
            }

            string[,] freqs = new string[inlist.Count, 2];//array with each character and count
            for(int i = 0; i < inlist.Count; i++)//filling in character section of array
            {
                freqs[i, 1] = "0";
                freqs[i, 0] = Convert.ToString(inlist.ElementAt(i));
            }


            for (int i = 0; i < inlist.Count; i++)//filling count section of array
            {
                foreach (char g in input)
                {
                    if (inlist.ElementAt(i) == g)
                    {
                        freqs[i, 1] = Convert.ToString(Convert.ToInt32(freqs[i, 1]) + 1);
                    }
                }
            }

            string[] freq1 = new string[inlist.Count];//arrays to sort freqs by count
            int[] freq2 = new int[inlist.Count];

            for (int i = 0; i < inlist.Count; i++)//populating arrays
            {
                freq1[i] = freqs[i, 0];
                freq2[i] = Convert.ToInt32(freqs[i, 1]);
            }

            Array.Sort(freq2, freq1);//sorting by count

            for (int i = 0; i < inlist.Count; i++)
            {
                freqs[i, 0] = freq1[i];
                freqs[i, 1] = Convert.ToString(freq2[i]);
            }//repopulating array


            //used for debugging purposes
            /*for (int i = 0; i < inlist.Count; i++) 
            {
                Console.WriteLine("value:{0}, frequency:{1}", freqs[i, 0], freqs[i, 1]);
            }*/

            huffmantree.nodelist = new List<node>();//initialising tree

            for (int i = 0; i < inlist.Count; i++)
            {
                huffmantree.nodelist.Add(new node(Convert.ToInt32(freqs[i, 1]), freqs[i, 0]));//each value in array to tree
            }

            for(int i = 0; i < inlist.Count - 1; i++)//traversing list to find lowest nodes, then joins to one and makes other values N/A so they wont be reused.
            {
                node low = null;
                node low1 = null;
                foreach(node x in huffmantree.nodelist)
                {
                    if (!x.na)
                    {
                        if(low is null)
                        {
                            low = x;
                        } else if(x.weight <= low.weight)
                        {
                            low1 = low;
                            low = x;
                        }

                        if(low1 is null && x.weight > low.weight)
                        {
                            low1 = x;
                        }
                    }
                }
                if (low1 is null)//used when last variable is available or when nodes have the same value, creates and initialises new node
                {
                    huffmantree.nodelist.Add(new node(low.weight + huffmantree.nodelist[huffmantree.nodelist.Count - 1].weight, low.value + huffmantree.nodelist[huffmantree.nodelist.Count - 1].value, huffmantree.nodelist[huffmantree.nodelist.Count - 1], huffmantree.nodelist.Find(node => node == low)));
                    huffmantree.nodelist.Find(node => node == low).na = true;
                    huffmantree.nodelist.Find(node => node == low).father = huffmantree.nodelist[huffmantree.nodelist.Count - 1];
                }
                else
                {//initalises new nodes with set values and sets old nodes to N/A and sets father
                    huffmantree.nodelist.Add(new node(low.weight + low1.weight, low.value + low1.value, huffmantree.nodelist.Find(node => node == low), huffmantree.nodelist.Find(node => node == low1)));
                    huffmantree.nodelist.Find(node => node == low).na = true;
                    huffmantree.nodelist.Find(node => node == low).father = huffmantree.nodelist[huffmantree.nodelist.Count - 1];
                    huffmantree.nodelist.Find(node => node == low1).na = true;
                    huffmantree.nodelist.Find(node => node == low1).father = huffmantree.nodelist[huffmantree.nodelist.Count - 1];
                }
            }

            //used for debugging
            /*foreach (node y in huffmantree.nodelist)
            {
                Console.WriteLine("value: {0}, Weight: {1}, NA: {2}", y.value, y.weight, y.na);
            }*/

            string[,] encoded = new string[inlist.Count, 2];//output initialised
            node tmp = null;//declare node for comparison
            string outstr = "";//output string to be added to encoded
            //defines code for each node by traversing list
            for (int i = 0; i < inlist.Count; i++)
            {
                foreach(node u in huffmantree.nodelist)
                {
                    if(u.value == Convert.ToString(inlist[i]))
                    {
                        tmp = u;
                        while (tmp.father != null)
                        {
                            if(tmp.father.left == tmp)
                            {
                                outstr = outstr + "0";//if node is on left side, assign 0 value
                            }
                            if(tmp.father.right == tmp)
                            {
                                outstr = outstr + "1";//if node is on right side assign 1 value
                            }
                            tmp = tmp.father;
                        }
                        string actualoutput = "";
                        for (int r = outstr.Length - 1; r >= 0; r--)//reversing code because it calculated from the bottom up as opposed to the top down
                        {
                            actualoutput += outstr[r];
                        }
                        encoded[i, 0] = Convert.ToString(inlist[i]);
                        encoded[i, 1] = actualoutput;//adding values to encoded list
                        outstr = "";//reset out string for future use
                    }
                }
            }


            //used for debugging
            /*foreach (string f in encoded)
            {
                Console.WriteLine(f);

            }*/

            //sets code on each node for faster finding
            for (int i = 0; i < inlist.Count; i++)
            {
                foreach(node n in huffmantree.nodelist)
                {
                    if (n.value == encoded[i, 0])
                    {
                        n.code = encoded[i, 1];
                    }
                }
            }

            Vector<string> output = new Vector<string>();//output vector initialised and populated
            foreach (char inchar in input)
            {
                for(int t = 0; t < inlist.Count; t++)
                {
                    if(encoded[t, 0] == Convert.ToString(inchar))
                    {
                        output.Add(encoded[t, 1]);
                    }
                }
            }
            Console.WriteLine(output);//output written for debugging purposes
            return output;
        }

        public Vector<char> Decode(Vector<string> input)
        {
            Vector<char> decoded = new Vector<char>();//outputs based on code per node
            foreach(string i in input)
            {
                foreach(node n in huffmantree.nodelist)
                {
                    if(n.code is null)
                    {

                    } else if (i == n.code)
                    {
                        decoded.Add(Convert.ToChar(n.value));
                    }
                }
            }


            //below code traverses the list everytime and is less effective than the above code
            /*node tmp = null;
            string outstr = "";
            foreach(string f in input)
            {
                outstr = "";
                tmp = huffmantree.nodelist[huffmantree.nodelist.Count - 1];
                foreach(char t in f)
                {
                    bool end = true;
                    while (end)
                    {
                        if (t == Convert.ToChar("0"))
                        {
                            if(tmp.left is null) { }
                            else
                            {
                                tmp = tmp.left;
                                Console.Write("value: {0}, freq: {1}    ", tmp.value, tmp.weight);
                            }
                        }
                        if (t == Convert.ToChar("1"))
                        {
                            if (tmp.right is null) { }
                            else
                            {
                                tmp = tmp.right;
                                Console.Write("value: {0}, freq: {1}    ", tmp.value, tmp.weight);
                            }
                        }
                        if (tmp.left is null || tmp.right is null)
                        {
                            decoded.Add(Convert.ToChar(tmp.value));
                            end = false;
                            Console.WriteLine();
                        }
                    }
                }
            }*/

            //give feedback to user that stuff is happing
            foreach(char g in decoded)
            {
                Console.Write(g + ", ");
            }
            Console.WriteLine();//cleans up output
            return decoded;
        }
    }
}
