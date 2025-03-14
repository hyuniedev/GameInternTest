using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public enum eMatchDirection
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    private int boardSizeX;

    private int boardSizeY;

    private Cell[,] m_cells;

    private Cell[] m_box;

    private Transform m_root;

    private int m_matchMin;

    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        m_matchMin = gameSettings.MatchesMin;

        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;

        m_cells = new Cell[boardSizeX, boardSizeY];
        m_box = new Cell[5];

        CreateBoard();
    }

    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);

                m_cells[x, y] = cell;
            }
        }

        Vector3 posBox = new Vector3(-5*0.5f+2, origin.y, 0f);
        for (int i = 0; i < 5; i++)
        {
            GameObject go = GameObject.Instantiate(prefabBG);
            go.transform.position = origin + new Vector3(posBox.x + i, (int)posBox.y, 0f);
            go.transform.SetParent(m_root);

            Cell cell = go.GetComponent<Cell>();
            cell.Setup((int)posBox.x + i, (int)posBox.y);
            cell.IsOnBox = true;

            m_box[i] = cell;
        }


        //set neighbours
        //for (int x = 0; x < boardSizeX; x++)
        //{
        //    for (int y = 0; y < boardSizeY; y++)
        //    {
        //        if (y + 1 < boardSizeY) m_cells[x, y].NeighbourUp = m_cells[x, y + 1];
        //        if (x + 1 < boardSizeX) m_cells[x, y].NeighbourRight = m_cells[x + 1, y];
        //        if (y > 0) m_cells[x, y].NeighbourBottom = m_cells[x, y - 1];
        //        if (x > 0) m_cells[x, y].NeighbourLeft = m_cells[x - 1, y];
        //    }
        //}

    }

    internal void InsertToBox(Item item)
    {
        for(int x = 0; x < 5; x++)
        {
            if (m_box[x].IsEmpty)
            {
                m_box[x].Assign(item);
                break;
            }
            else if (m_box[x].Item.IsSameType(item))
            {
                int y = x + 1;
                while(!m_box[y].IsEmpty) y++;
                for (; y > x; y--)
                {
                    m_box[y].Assign(m_box[y - 1].Item);
                }
                m_box[x].Assign(item);
                break;
            }
        }
        for (int x = 0; x < 3; x++)
        {
            if (m_box[x].IsSameType(m_box[x+1]) && m_box[x].IsSameType(m_box[x + 2]))
            {
                m_box[x].Clear();
                m_box[x + 1].Clear();
                m_box[x + 2].Clear();
                var index = x;
                for (int y = x + 3; y < 5; y++)
                {
                    if (!m_box[y].IsEmpty)
                    {
                        m_box[index].Assign(m_box[y].Item);
                        m_box[y].Free();
                        index++;
                    }
                }
            }
        }
    }
    private bool stop = false;
    internal void CheckEndGame(GameManager gameManager)
    {
        var endGame = true;
        foreach (var cell in m_cells)
        {
            if (!cell.IsEmpty) { endGame = false; }
        }
        if (endGame)
        {
            gameManager.SetState(GameManager.eStateGame.GAME_WIN);
            stop = true;
            return;
        }
        if (gameManager.GetLevelMode() == GameManager.eLevelMode.TIMER) return;
        foreach (var cell in m_box)
        {
            if (cell.IsEmpty) return;
        }
        gameManager.SetState(GameManager.eStateGame.GAME_OVER);
        stop = true;
    }
    internal void Fill()
    {
        List<Cell> lsCell = new List<Cell>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                lsCell.Add(m_cells[x, y]);
                //Cell cell = m_cells[x, y];
                //NormalItem item = new NormalItem();

                //List<NormalItem.eNormalType> types = new List<NormalItem.eNormalType>();
                //if (cell.NeighbourBottom != null)
                //{
                //    NormalItem nitem = cell.NeighbourBottom.Item as NormalItem;
                //    if (nitem != null)
                //    {
                //        types.Add(nitem.ItemType);
                //    }
                //}

                //if (cell.NeighbourLeft != null)
                //{
                //    NormalItem nitem = cell.NeighbourLeft.Item as NormalItem;
                //    if (nitem != null)
                //    {
                //        types.Add(nitem.ItemType);
                //    }
                //}

                //item.SetType(Utils.GetRandomNormalTypeExcept(types.ToArray()));
                //item.SetView();
                //item.SetViewRoot(m_root);

                //cell.Assign(item);
                //cell.ApplyItemPosition(false);
            }
        }
        var i = 0;
        while(i < lsCell.Count)
        {
            NormalItem item = new NormalItem();
            item.SetType(Utils.GetRandomNormalTypeExcept());
            item.SetView();
            item.SetViewRoot(m_root);
            lsCell[i].Assign(item);
            lsCell[i].ApplyItemPosition(false);
            i++;
            NormalItem item1 = new NormalItem();
            item1.SetType(item.ItemType);
            item1.SetView();
            item1.SetViewRoot(m_root);
            lsCell[i].Assign(item1);
            lsCell[i].ApplyItemPosition(false);
            i++;
            NormalItem item2 = new NormalItem();
            item2.SetType(item.ItemType);
            item2.SetView();
            item2.SetViewRoot(m_root);
            lsCell[i].Assign(item2);
            lsCell[i].ApplyItemPosition(false);
            i++;
        }
    }

    internal void SetInitCell()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                m_cells[x, y].Item.SetInitCell(m_cells[x, y]);
            }
        }
    }

    internal void Shuffle()
    {
        List<Item> list = new List<Item>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                list.Add(m_cells[x, y].Item);
                m_cells[x, y].Free();
            }
        }

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                int rnd = UnityEngine.Random.Range(0, list.Count);
                m_cells[x, y].Assign(list[rnd]);
                m_cells[x, y].ApplyItemMoveToPosition();

                list.RemoveAt(rnd);
            }
        }
    }
    internal IEnumerator AutoWin(GameManager gameManager)
    {
        for(int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                if(cell.IsEmpty) continue;
                InsertToBox(cell.Item);
                yield return new WaitForSeconds(0.5f);
                for (int k = 0; k<boardSizeX; k++)
                {
                    for(int l=0; l<boardSizeY; l++)
                    {
                        if (m_cells[k, l].IsEmpty) continue;
                        if (!(k==x && l==y))
                        {
                            if (cell.Item.IsSameType(m_cells[k, l].Item))
                            {
                                InsertToBox(m_cells[k, l].Item);
                                m_cells[k, l].Free();
                                yield return new WaitForSeconds(0.5f);
                                CheckEndGame(gameManager);
                                if (stop) yield break;
                            }
                        }
                    }
                }
                cell.Free();
                CheckEndGame(gameManager);
            }
        }
    }

    internal IEnumerator AutoLose(GameManager gameManager)
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                if (cell.IsEmpty) continue;
                int dem = 0;
                for(int k = 0; k< 5; k++)
                {
                    if(cell.Item.IsSameType(m_box[k].Item))
                    {
                        dem++;
                    }
                }
                if(dem < 2)
                {
                    InsertToBox(cell.Item);
                    cell.Free();
                    
                    yield return new WaitForSeconds(0.5f);
                    CheckEndGame(gameManager);
                    if (stop) yield break;
                }
            }
        }
    }

    //internal void FillGapsWithNewItems()
    //{
    //    for (int x = 0; x < boardSizeX; x++)
    //    {
    //        for (int y = 0; y < boardSizeY; y++)
    //        {
    //            Cell cell = m_cells[x, y];
    //            if (!cell.IsEmpty) continue;

    //            NormalItem item = new NormalItem();

    //            item.SetType(Utils.GetRandomNormalType());
    //            item.SetView();
    //            item.SetViewRoot(m_root);

    //            cell.Assign(item);
    //            cell.ApplyItemPosition(true);
    //        }
    //    }
    //}

    //internal void ExplodeAllItems()
    //{
    //    for (int x = 0; x < boardSizeX; x++)
    //    {
    //        for (int y = 0; y < boardSizeY; y++)
    //        {
    //            Cell cell = m_cells[x, y];
    //            cell.ExplodeItem();
    //        }
    //    }
    //}

    //public void Swap(Cell cell1, Cell cell2, Action callback)
    //{
    //    Item item = cell1.Item;
    //    cell1.Free();
    //    Item item2 = cell2.Item;
    //    cell1.Assign(item2);
    //    cell2.Free();
    //    cell2.Assign(item);

    //    item.View.DOMove(cell2.transform.position, 0.3f);
    //    item2.View.DOMove(cell1.transform.position, 0.3f).OnComplete(() => { if (callback != null) callback(); });
    //}

    //public List<Cell> GetHorizontalMatches(Cell cell)
    //{
    //    List<Cell> list = new List<Cell>();
    //    list.Add(cell);

    //    //check horizontal match
    //    Cell newcell = cell;
    //    while (true)
    //    {
    //        Cell neib = newcell.NeighbourRight;
    //        if (neib == null) break;

    //        if (neib.IsSameType(cell))
    //        {
    //            list.Add(neib);
    //            newcell = neib;
    //        }
    //        else break;
    //    }

    //    newcell = cell;
    //    while (true)
    //    {
    //        Cell neib = newcell.NeighbourLeft;
    //        if (neib == null) break;

    //        if (neib.IsSameType(cell))
    //        {
    //            list.Add(neib);
    //            newcell = neib;
    //        }
    //        else break;
    //    }

    //    return list;
    //}


    //public List<Cell> GetVerticalMatches(Cell cell)
    //{
    //    List<Cell> list = new List<Cell>();
    //    list.Add(cell);

    //    Cell newcell = cell;
    //    while (true)
    //    {
    //        Cell neib = newcell.NeighbourUp;
    //        if (neib == null) break;

    //        if (neib.IsSameType(cell))
    //        {
    //            list.Add(neib);
    //            newcell = neib;
    //        }
    //        else break;
    //    }

    //    newcell = cell;
    //    while (true)
    //    {
    //        Cell neib = newcell.NeighbourBottom;
    //        if (neib == null) break;

    //        if (neib.IsSameType(cell))
    //        {
    //            list.Add(neib);
    //            newcell = neib;
    //        }
    //        else break;
    //    }

    //    return list;
    //}

    //internal void ConvertNormalToBonus(List<Cell> matches, Cell cellToConvert)
    //{
    //    eMatchDirection dir = GetMatchDirection(matches);

    //    BonusItem item = new BonusItem();
    //    switch (dir)
    //    {
    //        case eMatchDirection.ALL:
    //            item.SetType(BonusItem.eBonusType.ALL);
    //            break;
    //        case eMatchDirection.HORIZONTAL:
    //            item.SetType(BonusItem.eBonusType.HORIZONTAL);
    //            break;
    //        case eMatchDirection.VERTICAL:
    //            item.SetType(BonusItem.eBonusType.VERTICAL);
    //            break;
    //    }

    //    if (item != null)
    //    {
    //        if (cellToConvert == null)
    //        {
    //            int rnd = UnityEngine.Random.Range(0, matches.Count);
    //            cellToConvert = matches[rnd];
    //        }

    //        item.SetView();
    //        item.SetViewRoot(m_root);

    //        cellToConvert.Free();
    //        cellToConvert.Assign(item);
    //        cellToConvert.ApplyItemPosition(true);
    //    }
    //}


    //internal eMatchDirection GetMatchDirection(List<Cell> matches)
    //{
    //    if (matches == null || matches.Count < m_matchMin) return eMatchDirection.NONE;

    //    var listH = matches.Where(x => x.BoardX == matches[0].BoardX).ToList();
    //    if (listH.Count == matches.Count)
    //    {
    //        return eMatchDirection.VERTICAL;
    //    }

    //    var listV = matches.Where(x => x.BoardY == matches[0].BoardY).ToList();
    //    if (listV.Count == matches.Count)
    //    {
    //        return eMatchDirection.HORIZONTAL;
    //    }

    //    if (matches.Count > 5)
    //    {
    //        return eMatchDirection.ALL;
    //    }

    //    return eMatchDirection.NONE;
    //}

    //internal List<Cell> FindFirstMatch()
    //{
    //    List<Cell> list = new List<Cell>();

    //    for (int x = 0; x < boardSizeX; x++)
    //    {
    //        for (int y = 0; y < boardSizeY; y++)
    //        {
    //            Cell cell = m_cells[x, y];

    //            var listhor = GetHorizontalMatches(cell);
    //            if (listhor.Count >= m_matchMin)
    //            {
    //                list = listhor;
    //                break;
    //            }

    //            var listvert = GetVerticalMatches(cell);
    //            if (listvert.Count >= m_matchMin)
    //            {
    //                list = listvert;
    //                break;
    //            }
    //        }
    //    }

    //    return list;
    //}

    //public List<Cell> CheckBonusIfCompatible(List<Cell> matches)
    //{
    //    var dir = GetMatchDirection(matches);

    //    var bonus = matches.Where(x => x.Item is BonusItem).FirstOrDefault();
    //    if(bonus == null)
    //    {
    //        return matches;
    //    }

    //    List<Cell> result = new List<Cell>();
    //    switch (dir)
    //    {
    //        case eMatchDirection.HORIZONTAL:
    //            foreach (var cell in matches)
    //            {
    //                BonusItem item = cell.Item as BonusItem;
    //                if (item == null || item.ItemType == BonusItem.eBonusType.HORIZONTAL)
    //                {
    //                    result.Add(cell);
    //                }
    //            }
    //            break;
    //        case eMatchDirection.VERTICAL:
    //            foreach (var cell in matches)
    //            {
    //                BonusItem item = cell.Item as BonusItem;
    //                if (item == null || item.ItemType == BonusItem.eBonusType.VERTICAL)
    //                {
    //                    result.Add(cell);
    //                }
    //            }
    //            break;
    //        case eMatchDirection.ALL:
    //            foreach (var cell in matches)
    //            {
    //                BonusItem item = cell.Item as BonusItem;
    //                if (item == null || item.ItemType == BonusItem.eBonusType.ALL)
    //                {
    //                    result.Add(cell);
    //                }
    //            }
    //            break;
    //    }

    //    return result;
    //}

    //internal List<Cell> GetPotentialMatches()
    //{
    //    List<Cell> result = new List<Cell>();
    //    for (int x = 0; x < boardSizeX; x++)
    //    {
    //        for (int y = 0; y < boardSizeY; y++)
    //        {
    //            Cell cell = m_cells[x, y];

    //            //check right
    //            /* example *\
    //              * * * * *
    //              * * * * *
    //              * * * ? *
    //              * & & * ?
    //              * * * ? *
    //            \* example  */

    //            if (cell.NeighbourRight != null)
    //            {
    //                result = GetPotentialMatch(cell, cell.NeighbourRight, cell.NeighbourRight.NeighbourRight);
    //                if (result.Count > 0)
    //                {
    //                    break;
    //                }
    //            }

    //            //check up
    //            /* example *\
    //              * ? * * *
    //              ? * ? * *
    //              * & * * *
    //              * & * * *
    //              * * * * *
    //            \* example  */
    //            if (cell.NeighbourUp != null)
    //            {
    //                result = GetPotentialMatch(cell, cell.NeighbourUp, cell.NeighbourUp.NeighbourUp);
    //                if (result.Count > 0)
    //                {
    //                    break;
    //                }
    //            }

    //            //check bottom
    //            /* example *\
    //              * * * * *
    //              * & * * *
    //              * & * * *
    //              ? * ? * *
    //              * ? * * *
    //            \* example  */
    //            if (cell.NeighbourBottom != null)
    //            {
    //                result = GetPotentialMatch(cell, cell.NeighbourBottom, cell.NeighbourBottom.NeighbourBottom);
    //                if (result.Count > 0)
    //                {
    //                    break;
    //                }
    //            }

    //            //check left
    //            /* example *\
    //              * * * * *
    //              * * * * *
    //              * ? * * *
    //              ? * & & *
    //              * ? * * *
    //            \* example  */
    //            if (cell.NeighbourLeft != null)
    //            {
    //                result = GetPotentialMatch(cell, cell.NeighbourLeft, cell.NeighbourLeft.NeighbourLeft);
    //                if (result.Count > 0)
    //                {
    //                    break;
    //                }
    //            }

    //            /* example *\
    //              * * * * *
    //              * * * * *
    //              * * ? * *
    //              * & * & *
    //              * * ? * *
    //            \* example  */
    //            Cell neib = cell.NeighbourRight;
    //            if (neib != null && neib.NeighbourRight != null && neib.NeighbourRight.IsSameType(cell))
    //            {
    //                Cell second = LookForTheSecondCellVertical(neib, cell);
    //                if (second != null)
    //                {
    //                    result.Add(cell);
    //                    result.Add(neib.NeighbourRight);
    //                    result.Add(second);
    //                    break;
    //                }
    //            }

    //            /* example *\
    //              * * * * *
    //              * & * * *
    //              ? * ? * *
    //              * & * * *
    //              * * * * *
    //            \* example  */
    //            neib = null;
    //            neib = cell.NeighbourUp;
    //            if (neib != null && neib.NeighbourUp != null && neib.NeighbourUp.IsSameType(cell))
    //            {
    //                Cell second = LookForTheSecondCellHorizontal(neib, cell);
    //                if (second != null)
    //                {
    //                    result.Add(cell);
    //                    result.Add(neib.NeighbourUp);
    //                    result.Add(second);
    //                    break;
    //                }
    //            }
    //        }

    //        if (result.Count > 0) break;
    //    }

    //    return result;
    //}

    //private List<Cell> GetPotentialMatch(Cell cell, Cell neighbour, Cell target)
    //{
    //    List<Cell> result = new List<Cell>();

    //    if (neighbour != null && neighbour.IsSameType(cell))
    //    {
    //        Cell third = LookForTheThirdCell(target, neighbour);
    //        if (third != null)
    //        {
    //            result.Add(cell);
    //            result.Add(neighbour);
    //            result.Add(third);
    //        }
    //    }

    //    return result;
    //}

    //private Cell LookForTheSecondCellHorizontal(Cell target, Cell main)
    //{
    //    if (target == null) return null;
    //    if (target.IsSameType(main)) return null;

    //    //look right
    //    Cell second = null;
    //    second = target.NeighbourRight;
    //    if (second != null && second.IsSameType(main))
    //    {
    //        return second;
    //    }

    //    //look left
    //    second = null;
    //    second = target.NeighbourLeft;
    //    if (second != null && second.IsSameType(main))
    //    {
    //        return second;
    //    }

    //    return null;
    //}

    //private Cell LookForTheSecondCellVertical(Cell target, Cell main)
    //{
    //    if (target == null) return null;
    //    if (target.IsSameType(main)) return null;

    //    //look up        
    //    Cell second = target.NeighbourUp;
    //    if (second != null && second.IsSameType(main))
    //    {
    //        return second;
    //    }

    //    //look bottom
    //    second = null;
    //    second = target.NeighbourBottom;
    //    if (second != null && second.IsSameType(main))
    //    {
    //        return second;
    //    }

    //    return null;
    //}

    //private Cell LookForTheThirdCell(Cell target, Cell main)
    //{
    //    if (target == null) return null;
    //    if (target.IsSameType(main)) return null;

    //    //look up
    //    Cell third = CheckThirdCell(target.NeighbourUp, main);
    //    if (third != null)
    //    {
    //        return third;
    //    }

    //    //look right
    //    third = null;
    //    third = CheckThirdCell(target.NeighbourRight, main);
    //    if (third != null)
    //    {
    //        return third;
    //    }

    //    //look bottom
    //    third = null;
    //    third = CheckThirdCell(target.NeighbourBottom, main);
    //    if (third != null)
    //    {
    //        return third;
    //    }

    //    //look left
    //    third = null;
    //    third = CheckThirdCell(target.NeighbourLeft, main); ;
    //    if (third != null)
    //    {
    //        return third;
    //    }

    //    return null;
    //}

    //private Cell CheckThirdCell(Cell target, Cell main)
    //{
    //    if (target != null && target != main && target.IsSameType(main))
    //    {
    //        return target;
    //    }

    //    return null;
    //}

    //internal void ShiftDownItems()
    //{
    //    for (int x = 0; x < boardSizeX; x++)
    //    {
    //        int shifts = 0;
    //        for (int y = 0; y < boardSizeY; y++)
    //        {
    //            Cell cell = m_cells[x, y];
    //            if (cell.IsEmpty)
    //            {
    //                shifts++;
    //                continue;
    //            }

    //            if (shifts == 0) continue;

    //            Cell holder = m_cells[x, y - shifts];

    //            Item item = cell.Item;
    //            cell.Free();

    //            holder.Assign(item);
    //            item.View.DOMove(holder.transform.position, 0.3f);
    //        }
    //    }
    //}

    public void Clear()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                cell.Clear();

                GameObject.Destroy(cell.gameObject);
                m_cells[x, y] = null;
            }
        }
    }
}
